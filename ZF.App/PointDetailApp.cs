using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.App.Request;
using ZF.App.Response;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class PointDetailApp : BaseApp<PointDetail>
    {

        public IEnumerable<PointDetailOut> GetPointDetail(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@id", input.Id));
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@type", input.StoreType));
            sqlParamters.Add(new SqlParameter("@start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@end", input.End ?? ""));

            var r = Repository.ExecuteQuery<PointDetailOut>("EXEC sp_GetPagePointDetail @id,@pageindex,@pagesize,@type,@start,@end", sqlParamters.ToArray());

            return r;
        }
    }
}
