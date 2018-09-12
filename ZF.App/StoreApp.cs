using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZF.Repository.Domain;
using Infrastructure;
using System.Data.SqlClient;
using System.Data;
using ZF.App.Response;
using ZF.App.Request;
using System.Transactions;
namespace ZF.App
{
    public class StoreApp : BaseApp<Store>
    {
        public IEnumerable<StoreOut> GetStoreList(StoreCondition input, out int totalCount)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@storetype", input.StoreType));
            var outParameter = new SqlParameter("@count", SqlDbType.Int, 4);
            outParameter.Direction = ParameterDirection.Output;
            sqlParamters.Add(outParameter);

            var r = Repository.ExecuteQuery<StoreOut>("EXEC sp_GetPageStore @pageindex,@pagesize,@name,@storetype,@count out", sqlParamters.ToArray());
            totalCount = Convert.ToInt32(outParameter.Value == DBNull.Value ? 0 : outParameter.Value);

            return r;
        }

        public IEnumerable<JifenOut> GetStoreJifen(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));

            var r = Repository.ExecuteQuery<JifenOut>("EXEC sp_GetPageJifen @pageindex,@pagesize,@name", sqlParamters.ToArray());

            return r;
        }

        public Store CheckMobile(Store input)
        {
            return Repository.GetWhere(r => r.Id != input.Id && r.Mobile.Equals(input.Mobile.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public Store CheckDeviceId(Store input)
        {
            return Repository.GetWhere(r => r.DeviceId.Equals(input.DeviceId.ToUpper(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public string CreateQr(string name) {
            return CompanyPay.QRCodeHelper.CreateImg(name);
        }

        public bool Insert2(Store input)
        {
            Collect collect = new Collect() { Account = "", ALI = "", BANK = "", ALIPer = 0, BANKPer = 0, BANKType = "", CreateTime = DateTime.Now, WX = "", WXPer = 0 };
            using (var tran = new TransactionScope())
            {
                UnitWork.RegisterNew<Collect>(collect);
                UnitWork.Commit();
                input.CollectId = collect.Id;
                UnitWork.RegisterNew<Store>(input);
                UnitWork.Commit();
                UnitWork.RegisterNew<Account>(new Account() { StoreId = input.Id, StoreName = input.StoreName, Balance = 0, CreateTime = DateTime.Now, TakeOut = 0, TodayIn = 0, TotalIn = 0, IsActive= true });
                UnitWork.Commit();
                tran.Complete();
            }
            return true;
        }
        
        public bool Update2(Store input,int type = 0)
        {
            
            var config = Repository.Get(input.Id);
            if (config == null)
                throw new Exception("当前数据不存在");

            //修改商家名称
            if (!config.StoreName.Equals(input.StoreName)) {
                var sqlParamters = new List<SqlParameter>();
                sqlParamters.Add(new SqlParameter("@Id", input.Id));
                sqlParamters.Add(new SqlParameter("@StoreName", input.StoreName));
                Repository.ExecuteSql("EXEC sp_UpdateStoreName @Id,@StoreName", sqlParamters.ToArray());
            }

            if (type == 0)
            {
                Repository.Update(r => r.Id == input.Id, r => new Store
                {
                    StoreName = input.StoreName,
                    StoreType = input.StoreType,
                    Address = input.Address,
                    Province = input.Province,
                    City = input.City,
                    Area = input.Area,
                    PName = input.PName,
                    AName = input.AName,
                    CName = input.CName
                });
            }
            else if (type == 1)
            {
                Repository.Update(r => r.Id == input.Id, r => new Store() { IsActive = !config.IsActive});
            }
            return true;
        }
        
    }
}
