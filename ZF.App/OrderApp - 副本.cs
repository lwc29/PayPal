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
namespace ZF.App
{
    //收款账号
    public class OrderApp : BaseApp<Order>
    {
       // public TakeListApp takelistapp { get; set; }

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

        public string OutTradeNo(string amount, string storeid)
        {
            return CompanyPay.AliPay.OutTradeNo(amount, storeid);
        }

        public bool InsertTiXian(Order order,Collect collect, string storename, decimal balance,int aid,string accountid,decimal shouxufei, out string msg)
        {
            decimal per = 0;
            decimal tips = 0;
            string remake = string.Empty;
            if (order.PayType == 4)
                per = collect.ALIPer;
            else if (order.PayType == 5)
                per = collect.WXPer;

            
            if (per > 0)
                remake = "按商户手续费收取";
            else {
                per = shouxufei;
                remake = "按平台手续费收取";
            }

            tips = Math.Round(order.Total_amount * per, 2, MidpointRounding.AwayFromZero);
           
            using (var tran = new TransactionScope())
            {

                UnitWork.RegisterNew<TakeList>(new TakeList() { StoreId = order.StoreNo, StoreName = storename, Balance = balance, TakeOut = order.Total_amount, TakeType = order.PayType, CreateTime = DateTime.Now, Account = collect.Account, AccountId = accountid , Out_trade_no = order.Out_trade_no });
                UnitWork.RegisterNew<Order>(order);
                UnitWork.Commit();
                tran.Complete();
            }
            TipsFlow tf = new TipsFlow() { StoreId = order.StoreNo, StoreName = storename, Balance = balance, TakeOut = order.Total_amount, TakeType = order.PayType, CreateTime = DateTime.Now, Account = collect.Account, Tips = tips, Per = per, Remark = remake };
           return TiXian(order,collect,tf, storename, balance,aid,tf.Id, accountid, tips,out msg);
        }

        public bool TiXian(Order order, Collect collect, TipsFlow tf, string storename, decimal balance, int aid, int tfid, string accountid, decimal tips, out string msg)
        {
           
            CompanyPay.AliPay ali = new CompanyPay.AliPay();

            Aop.Api.Response.AlipayFundTransToaccountTransferResponse response = ali.GetCompanyPay(order.Out_trade_no, accountid, (order.Total_amount - tips).ToString(), storename, collect.Account, order.Remark);

            TakeList takelist = null;// takelistapp.Repository.GetWhere(r => r.Out_trade_no == order.Out_trade_no).FirstOrDefault();
            order = Repository.Get(order.Id);
            if (response.Code == "10000")
            {
                Account account = accountapp.Get(aid);
                TipsFlow tipsflow = tipsflowapp.Get(tfid);
                if (account == null)
                    throw new Exception("找不到该资金账户");
                using (var tran = new TransactionScope())
                {

                    UnitWork.RegisterNew<TipsFlow>(tf);
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
            else {  //失败
                using (var tran = new TransactionScope())
                {
                    UnitWork.RegisterDirty(takelist, () => new TakeList { State = 1 });
                    UnitWork.RegisterDirty(takelist, () =>  new TakeList { State = 2  });

                    UnitWork.RegisterDirty<Order>(order, () => new Order() {
                        Gmt_payment = DateTime.Now,
                       Trade_status = response.Code,
                       IsSuccess = 2,
                        //order.Trade_no = "";
                       Remark = response.SubMsg + "【" + response.SubCode+"】",
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
         
    }
}
