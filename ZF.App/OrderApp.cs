using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using System.Diagnostics;
using System.Web.Http;
using ZF.App.Request;
using ZF.App.Response;
using System.Data.SqlClient;
using System.Transactions;
using CompanyPay.Core;
using CompanyPay.Model;
namespace ZF.App
{
    //收款账号
    public class OrderApp : BaseApp<Order>
    {
        public TakeListApp takelistapp { get; set; }

        public TipsFlowApp tipsflowapp { get; set; }
        public AccountApp accountapp { get; set; }

        public IEnumerable<OrderOut> GetOrderList(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@trade_no", input.Area ?? ""));
            sqlParamters.Add(new SqlParameter("@Start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@End", input.End ?? ""));
            sqlParamters.Add(new SqlParameter("@Type", input.StoreType));

            var r = Repository.ExecuteQuery<OrderOut>("EXEC sp_GetPageOrder @pageindex,@pagesize,@type,@name,@trade_no,@Start,@End", sqlParamters.ToArray());

            return r;
        }

        public string OutTradeNo(string amount, string storeid, int type)
        {
            return CompanyPay.AliPay.OutTradeNo(amount, storeid.Substring(4), type);
        }

        public bool InsertTiXian(Order order, Collect collect, string storename, decimal balance, int aid, string accountid, decimal shouxufei, string pointrule, out string msg)
        {
            decimal per = 0;

            string remake = string.Empty;
            if (order.PayType == 4)
                per = collect.ALIPer / 100;
            else if (order.PayType == 5)
                per = collect.WXPer / 100;


            if (per > 0)
                remake = "按商户手续费收取";
            else
            {
                per = shouxufei;
                remake = "按平台手续费收取";
            }

            decimal tips = Math.Round(order.Total_amount * per + 0.009M, 2, MidpointRounding.AwayFromZero);
            if (tips - order.Total_amount * per == 0.01M)
                tips = Math.Round(order.Total_amount * per, 2, MidpointRounding.AwayFromZero);
            if (tips < 0.01M)
                throw new Exception("提现额度太低");
             if (order.PayType == 5 && order.Total_amount - tips < 1)
                throw new Exception("提现额度太低");
            //积分
            decimal point = 0;
            string[] args = pointrule.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (tips >= Convert.ToDecimal(args[2]))
            {
                point = Math.Floor(tips * Convert.ToDecimal(args[1]));
            }
            order.Tips = tips;
            TipsFlow tf = null;
            using (var tran = new TransactionScope())
            {

                UnitWork.RegisterNew<TakeList>(new TakeList() { StoreId = order.StoreNo, StoreName = storename, Balance = balance, TakeOut = order.Total_amount, TakeType = order.PayType, CreateTime = DateTime.Now, Account = collect.Account, AccountId = accountid, Out_trade_no = order.Out_trade_no });
                UnitWork.RegisterNew<Order>(order);
                tf = new TipsFlow() { StoreId = order.StoreNo, StoreName = storename, TakeOut = order.Total_amount, TakeType = order.PayType, CreateTime = DateTime.Now, Account = collect.Account, Tips = tips, Per = per, Remark = remake, IsActive = false };
                UnitWork.RegisterNew<TipsFlow>(tf);
                UnitWork.Commit();
                tran.Complete();
            }
            if (order.PayType == 4)
                return TiXian(order, tf, collect.Account, storename, balance, aid, accountid, tips, point, out msg);
            else
                return TiXianWX(order, tf, collect.Account, storename, balance, aid, accountid, tips, point, out msg);
        }

        public bool TiXian(Order order, TipsFlow tf, string accountnm, string storename, decimal balance, int aid, string accountid, decimal tips, decimal point, out string msg)
        {

            CompanyPay.AliPay ali = new CompanyPay.AliPay();
            
            Aop.Api.Response.AlipayFundTransToaccountTransferResponse response = ali.GetCompanyPay(order.Out_trade_no, accountid, (order.Total_amount - tips).ToString(), storename, accountnm, order.Remark);
            CompanyPay.LogHelper.WriteLine(Jayrock.Json.Conversion.JsonConvert.ExportToString(response));

            CompanyPay.LogHelper.WriteLine(response.Body);


            #region MyRegion
            try
            {
                TakeList takelist = takelistapp.Repository.GetWhere(r => r.Out_trade_no == order.Out_trade_no).FirstOrDefault();
                order = Repository.Get(order.Id);
                if (response.Code == "10000")
                {
                    Account account = accountapp.Get(aid);
                    if (account == null)
                        throw new Exception("找不到该资金账户");
                    using (var tran = new TransactionScope())
                    {
                        if (point >= 1)
                            UnitWork.RegisterNew(new PointDetail() { FromType = order.PayType, Money = order.Total_amount, CreateTime = DateTime.Now.ToLocalTime(), IsActive = true, Point = point, StoreId = order.StoreNo, StoreName = storename, BuyerId = accountid });

                        UnitWork.RegisterDirty(tf, () => new TipsFlow { IsActive = true });
                        UnitWork.RegisterDirty(takelist, () => new TakeList { State = 1 });

                        order.Gmt_payment = Convert.ToDateTime(response.PayDate);
                        order.Trade_status = response.Code;
                        order.IsSuccess = 1;
                        order.Trade_no = response.OrderId;
                        UnitWork.RegisterDirty<Order>(order);

                        account.Balance -= order.Total_amount;
                        account.TakeOut += order.Total_amount;
                        UnitWork.RegisterDirty<Account>(account);

                        UnitWork.Commit();
                        tran.Complete();
                    }
                }
                else
                {  //失败
                    using (var tran = new TransactionScope())
                    {
                        UnitWork.RegisterDirty(takelist, () => new TakeList { State = 2 });

                        UnitWork.RegisterDirty<Order>(order, () => new Order()
                        {
                            Gmt_payment = DateTime.Now,
                            Trade_status = response.Code,
                            IsSuccess = 2,
                            Remark = response.SubMsg + "【" + response.SubCode + "】",
                        });

                        UnitWork.Commit();
                        tran.Complete();
                    }
                }
                if (response.IsError)
                {
                    msg = response.SubMsg;
                    return false;
                }
                else
                {
                    msg = response.Msg;
                    return true;
                }
            }
            catch (Exception)
            {
                //记录这笔成功订单 写入JSON 
                if (response.Code == "10000")
                {
                    string orderstr = response.Body;
                    orderstr = orderstr.Substring(0, orderstr.LastIndexOf("sign") - 3).Replace("alipay_fund_trans_toaccount_transfer_response", "ResponseOrder");
                    orderstr += ",\"oid\":" + order.Id + ",\"tfid\":" + tf.Id + ",\"aid\":" + aid + ",\"point\":" + point + "}}";
                    OrderJson orderjson = ConfigFactory.GetConfig<OrderJson>();
                    SuccessOrder resOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<SuccessOrder>(orderstr);
                    if (orderjson == null || orderjson.list == null)
                    {
                        orderjson = new OrderJson();
                        orderjson.list = new List<SuccessOrder>();
                    }
                    orderjson.list.Add(new SuccessOrder { ResponseOrder = resOrder.ResponseOrder });
                    ConfigFactory.SetConfig<OrderJson>(orderjson);
                    msg = "支付成功,订单有误";

                }
                else
                    msg = response.SubMsg;
                return false;
            }
            #endregion

        }

        public bool TiXianWX(Order order, TipsFlow tf, string accountnm, string storename, decimal balance, int aid, string accountid, decimal tips, decimal point, out string msg)
        {
            CompanyPay.WxPayData paydata = CompanyPay.WX.GetCompanyPay(order.Out_trade_no, (order.Total_amount - tips).ToString(), accountid, accountnm, order.Remark);
            CompanyPay.LogHelper.WriteLine(Jayrock.Json.Conversion.JsonConvert.ExportToString(paydata.GetValues()));



            TakeList takelist = takelistapp.Repository.GetWhere(r => r.Out_trade_no == order.Out_trade_no).FirstOrDefault();
            order = Repository.Get(order.Id);

            if ((paydata.GetValue("return_code").ToString() == "SUCCESS" &&
            paydata.GetValue("result_code").ToString() == "SUCCESS"))
            {
                try
                {
                    Account account = accountapp.Get(aid);
                    if (account == null)
                        throw new Exception("找不到该资金账户");
                    using (var tran = new TransactionScope())
                    {
                        if (point >= 1)
                            UnitWork.RegisterNew(new PointDetail() { FromType = order.PayType, Money = order.Total_amount, CreateTime = DateTime.Now.ToLocalTime(), IsActive = true, Point = point, StoreId = order.StoreNo, StoreName = storename, BuyerId = accountid });

                        UnitWork.RegisterDirty(tf, () => new TipsFlow { IsActive = true });
                        UnitWork.RegisterDirty(takelist, () => new TakeList { State = 1 });

                        order.Gmt_payment = Convert.ToDateTime(paydata.GetValue("payment_time").ToString());
                        order.Trade_status = "SUCCESS";
                        order.IsSuccess = 1;
                        order.Trade_no = paydata.GetValue("payment_no").ToString();
                        UnitWork.RegisterDirty<Order>(order);

                        account.Balance -= order.Total_amount;
                        account.TakeOut += order.Total_amount;
                        UnitWork.RegisterDirty<Account>(account);

                        UnitWork.Commit();
                        tran.Complete();
                    }

                    msg = paydata.GetValue("result_code").ToString();
                    return true;

                }
                catch (Exception ex)
                {
                    msg = "支付成功,订单有误";
                    //记录这笔成功订单 写入JSON 
                    return false;
                }
            }
            else
            {
                //失败
                using (var tran = new TransactionScope())
                {
                    UnitWork.RegisterDirty(takelist, () => new TakeList { State = 2 });

                    UnitWork.RegisterDirty<Order>(order, () => new Order()
                    {
                        Gmt_payment = DateTime.Now.ToLocalTime(),
                        Trade_status = paydata.GetValue("result_code").ToString(),
                        IsSuccess = 2,
                        Remark = paydata.GetValue("err_code").ToString() + "【" + paydata.GetValue("err_code_des").ToString() + "】",
                    });

                    UnitWork.Commit();
                    tran.Complete();
                }

                msg = paydata.GetValue("result_code").ToString();
                return false;
            }
        }

        public List<SuccessOrderOut> GetTakeListEx()
        {
            OrderJson order = ConfigFactory.GetConfig<OrderJson>() ?? new OrderJson();
            List<SuccessOrderOut> so = new List<SuccessOrderOut>();
            if (order != null && order.list.Count == 0)
                return so;
            foreach (var item in order.list)
            {
                so.Add(new SuccessOrderOut() { ResponseOrder = item.ResponseOrder });
            }
            return so;
        }

        public bool UpdateEx(Order input)
        {
            OrderJson orderjson = ConfigFactory.GetConfig<OrderJson>() ?? new OrderJson();
            List<SuccessOrder> list = orderjson.list;
            if (list == null || list.Count == 0)
                return false;
            SuccessOrder success = list.FirstOrDefault(r => r.ResponseOrder.out_biz_no == input.Out_trade_no);
            var flag = false;
            if (success != null)
            {
                ResponseOrder res = success.ResponseOrder;
                TakeList takelist = takelistapp.Repository.GetWhere(r => r.Out_trade_no == res.out_biz_no).FirstOrDefault();
                Order order = Repository.Get(res.oid);
                if (order == null || order.Out_trade_no != input.Out_trade_no)
                    throw new MsgException("文件信息有误.");
                if (order.IsSuccess != 0)
                    throw new MsgException("订单已经改写,不能覆盖.");

                Account account = accountapp.Get(res.aid);
                TipsFlow tf = tipsflowapp.Get(res.tfid);
                if (tf == null || tf.IsActive == true)
                    throw new MsgException("手续费信息有误");
                if (account == null)
                    throw new MsgException("找不到该资金账户");
                using (var tran = new TransactionScope())
                {
                    if (res.point >= 1)
                        UnitWork.RegisterNew(new PointDetail() { FromType = order.PayType, Money = order.Total_amount, CreateTime = DateTime.Now, IsActive = true, Point = res.point, StoreId = order.StoreNo, StoreName = takelist.StoreName, BuyerId = takelist.Account });

                    UnitWork.RegisterDirty(tf, () => new TipsFlow { IsActive = true });
                    UnitWork.RegisterDirty(takelist, () => new TakeList { State = 1 });

                    order.Gmt_payment = Convert.ToDateTime(res.pay_date);
                    order.Trade_status = res.code;
                    order.IsSuccess = 1;
                    order.Trade_no = res.order_id;
                    UnitWork.RegisterDirty<Order>(order);

                    account.Balance -= order.Total_amount;
                    account.TakeOut += order.Total_amount;
                    UnitWork.RegisterDirty<Account>(account);

                    UnitWork.Commit();
                    tran.Complete();
                }
                list.Remove(success);
                //订单Json文件修改.
                ConfigFactory.SetConfig<OrderJson>(orderjson);

                flag = true;
            }
            else
                throw new MsgException("未找到该订单信息");
            return flag;
        }

    }
}
