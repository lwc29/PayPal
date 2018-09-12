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
using CompanyPay.Core;
using CompanyPay.Model;
namespace ZF.App
{
    /// <summary>
    /// 体现列表
    /// </summary>
    public class TakeListApp : BaseApp<TakeList>
    {
        public IEnumerable<TakeListOut> GetTakeList(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@Start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@End", input.End ?? ""));

            var r = Repository.ExecuteQuery<TakeListOut>("EXEC sp_GetPageTakeList @pageindex,@pagesize,@name,@Start,@End", sqlParamters.ToArray());

            return r;
        }

        public bool ExistTiXian(string accountId)
        {
            return Repository.GetWhere(r => r.CreateTime.ToShortDateString() == DateTime.Now.ToShortDateString() && r.AccountId == accountId).Count() > 2 ? false : true;
        }

       
     }
}
