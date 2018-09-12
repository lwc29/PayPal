using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using ZF.App.Response;
using ZF.App.Request;
using ZF.App.SSO;
namespace ZF.App
{
    public class ModuleApp : BaseApp<Module>
    {
        public List<ModuleOut> GetAll(string token)
        {
           
            var cache = CacheHelper.Get<IEnumerable<Module>>("Module_" + token);
            IEnumerable<Module> list = null;

            if (cache == null)
            { 
                //list = Repository.GetAll();
                list = Repository.ExecuteQuery<Module>(string.Format(@"WITH t1
                                                                        AS(
	                                                                        SELECT m.ParentId FROM dbo.Module m JOIN dbo.RoleAndMoudule rm ON rm.ModuleId = m.Id  
	                                                                        WHERE RoleId = {0}
	                                                                        GROUP BY m.ParentId
                                                                        )SELECT m.* FROM t1 JOIN dbo.Module m ON t1.ParentId = m.Id
                                                                        UNION 
                                                                        SELECT  m.*
                                                                            FROM dbo.Module m
                                                                            INNER JOIN dbo.RoleAndMoudule  rm 
                                                                            ON m.Id = rm.ModuleId 
                                                                            WHERE  rm.RoleId = {0}", token));
                CacheHelper.Set<IEnumerable<Module>>("Module_" + token, list, DateTime.Now.ToLocalTime().AddDays(1));
            }
            else
                list = cache;
           
            List<ModuleOut> newList = new List<ModuleOut>();
            if (list != null)
            {
                list = list.OrderBy(r => r.SortNo);
                foreach (var item in list.Where(r => r.ParentId == 0))
                {
                    var mo = item.MapTo<ModuleOut>();
                    var lst = list.Where(r => r.ParentId == item.Id).OrderBy(r => r.SortNo);
                    mo.Children.AddRange(lst);
                    newList.Add(mo);
                }
            }
            return newList;
        }
    }
}
