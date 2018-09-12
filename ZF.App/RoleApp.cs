using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using System.Diagnostics;
using System.Data.SqlClient;
namespace ZF.App
{
    public class RoleApp : BaseApp<Role>
    {
        public void Update(Role input)
        {
            Repository.Update(r => r.Id == input.Id, r => new Role() { RoleName= input.RoleName, Remark = input.Remark });
        }

        public void Del(Role input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@id", input.Id));

            var r = Repository.ExecuteSql("EXEC sp_DelRole @id", sqlParamters.ToArray());
        }
 
    }
}
