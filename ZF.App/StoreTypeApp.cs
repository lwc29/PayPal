using Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class StoreTypeApp : BaseApp<StoreType>
    {
        public IEnumerable<StoreType> GetAll()
        {
            var list = Repository.GetAll();
            StaticHelper.stStoreType = list;
            //CacheHelper.Set<IEnumerable<StoreType>>("StoreType", list, DateTime.Now.AddDays(7));
            return list;
        }
    }
}
