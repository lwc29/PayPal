using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;

namespace ZF.App
{
   public static class StaticHelper
    {
       public static IEnumerable<StoreType> stStoreType { get; internal set; }
    }
}
