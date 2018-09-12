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
using ZF.App.SSO;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class UserApp : BaseApp<User>
    {
       
        public User GetUser(User user)
        {
            var result = Repository.GetWhere(r => r.Tel == user.Tel || r.UserName.Equals(user.Tel,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return result;
        }

        public User GetCacheUser(User user)
        {
            var ur = CacheHelper.Get<User>(AuthUtil._info + user.UserName);
            if (ur != null)
                return ur;
            return GetUser(user);
        }

        public IEnumerable<UserOut> GetUsersList(UserCondition input, out int totalCount)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@email", input.Email ?? ""));
            var outParameter = new SqlParameter("@count", SqlDbType.Int, 4);
            outParameter.Direction = ParameterDirection.Output;
            sqlParamters.Add(outParameter);

            var r = Repository.ExecuteQuery<UserOut>("EXEC sp_GetPageUsers @pageindex,@pagesize,@name,@email,@count out", sqlParamters.ToArray());
            totalCount = Convert.ToInt32(outParameter.Value == DBNull.Value ? 0 : outParameter.Value);

            return r;
        }

        public bool Update(User input, int type = 0)
        {

            var config = Repository.Get(input.Id);
            if (config == null)
                throw new Exception("当前数据不存在");
            if (config.UserName == "admin" && type == 2)
                throw new Exception("当前数据不能修改");
            
            //if (!config.StoreName.Equals(input.StoreName))
            //{
            //    var sqlParamters = new List<SqlParameter>();
            //    sqlParamters.Add(new SqlParameter("@Id", input.Id));
            //    sqlParamters.Add(new SqlParameter("@StoreName", input.StoreName));
            //    Repository.ExecuteSql("EXEC sp_UpdateStoreName @Id,@StoreName", sqlParamters.ToArray());
            //}

            if (type == 0)
            {
                Repository.Update(config, () => new User { Pwd = config.Pwd },true);
                //Repository.Update(r => r.Id == input.Id, r => new Store
                //{
                //    StoreName = input.StoreName,
                //    StoreType = input.StoreType,
                //    Address = input.Address,
                //    Province = input.Province,
                //    City = input.City,
                //    Area = input.Area,
                //    PName = input.PName,
                //    AName = input.AName,
                //    CName = input.CName
                //});
            }
            else if (type == 1)
            {
                Repository.Update(r => r.Id == input.Id, r => new User() { IsActive = !config.IsActive });
            }
            else if (type == 2)
            { 
                Repository.Update(r => r.Id == input.Id, r => new User() { RoleId = input.RoleId });
                config.RoleId = input.RoleId;
                CacheHelper.Set<User>(AuthUtil._info + config.UserName,config, DateTime.Now.ToLocalTime().AddMinutes(30));
            }
            return true;
        }

       
    }
}
