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
namespace ZF.App
{
    /// <summary>
    /// 手续费流水
    /// </summary>
    public class TipsFlowApp : BaseApp<TipsFlow>
    {
        public IEnumerable<TipsFlowOut> GetTipsFlowList(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@Start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@End", input.End ?? ""));

            var r = Repository.ExecuteQuery<TipsFlowOut>("EXEC sp_GetPageTipsFlow @pageindex,@pagesize,@name,@Start,@End", sqlParamters.ToArray());

            return r;
        }
         
    }
}
