using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class SettingApp : BaseApp<Settings>
    {
        public Settings GetSettingsByName(Settings input)
        {
            return Repository.GetWhere(r => r.Name.Equals(input.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
