using Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.App.Request;
using ZF.App.Response;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class RoleAndMouduleApp : BaseApp<RoleAndMoudule>
    {
        public IEnumerable<RoleAndMouduleOut> GetAccess(int  rid)
        {
            string sql = string.Format(@"SELECT m.Id,
                        m.title, 
                        CAST( CASE WHEN rm.RoleId IS NOT NULL THEN 1 ELSE 0 END AS BIT ) checked,
                        CAST( CASE WHEN (m.Title ='角色设置' AND rm.RoleId = 2) THEN 1 ELSE 0 END AS  Bit) disabled,
	                    m.ParentId,
						m.SortNo
                        FROM dbo.Module m
                        LEFT JOIN dbo.RoleAndMoudule  rm ON m.Id = rm.ModuleId AND rm.RoleId = {0}", rid);

           var list = Repository.ExecuteQuery<RoleAndMouduleOut>(sql);
        
           List<RoleAndMouduleOut> newList = new List<RoleAndMouduleOut>();
           if (list != null)
           {
               list = list.OrderBy(r => r.SortNo);
               foreach (var item in list.Where(r => r.ParentId == 0))
               {
               
                   var lst = list.Where(r => r.ParentId == item.Id).OrderBy(r => r.SortNo);
                   item.data.AddRange(lst);
                   newList.Add(item);
               }
           }
           return newList;
        }


        public IEnumerable<RoleAndMouduleOut> GetPersonAccess(int rid)
        {
            string sql = string.Format(@" SELECT u.Id,UserName +'(' +u.Tel + ')' title ,RoleId, 
                                        CAST( CASE WHEN u.RoleId = {0} THEN 1 ELSE 0 END AS BIT ) checked,
                                        CAST( CASE WHEN u.UserName = 'admin' THEN 1 ELSE 0 END as Bit) disabled
                                        FROM dbo.Users u
                                        LEFT JOIN DBO.Role r ON u.RoleId = r.Id
                                        WHERE IsActive = 1", rid);

            var list = Repository.ExecuteQuery<RoleAndMouduleOut>(sql);
            //List<RoleAndMouduleOut> newList = new List<RoleAndMouduleOut>();
            //if (list != null)
            //{
            //    list = list.OrderBy(r => r.SortNo);
            //    foreach (var item in list.Where(r => r.ParentId == 0))
            //    {

            //        var lst = list.Where(r => r.ParentId == item.Id).OrderBy(r => r.SortNo);
            //        item.data.AddRange(lst);
            //        newList.Add(item);
            //    }
            //}
            return list;
        }

        public int UpdateAccess(RoleAndMouduleIn input)
        {
            var sqlParamters = new List<SqlParameter>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            if (input.RoleId == 2) {
                if (input.Ids.IndexOf("24") == -1)
                    input.Ids.Add("24");
                if (input.Ids.IndexOf("25") == -1)
                    input.Ids.Add("25");
            }

            foreach (var item in input.Ids)
            {
                dt.Rows.Add(item);
            }

            SqlParameter param = new SqlParameter("@Ids", SqlDbType.Structured);
            param.Value = dt;
            param.TypeName = "Ids";
            sqlParamters.Add(param);
            sqlParamters.Add(new SqlParameter("@RoleId", input.RoleId));
            var r = Repository.ExecuteSql("EXEC sp_UpdateRoleAndMoudule @Ids, @RoleId", sqlParamters.ToArray());
            CacheHelper.Remove("Module_" + input.RoleId);
            return r;
        }
    }
}
