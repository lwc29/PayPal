using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;

namespace ZF.App
{
    public class AuthoriseApp : BaseApp<Module>
    {
        protected User _user;

        private List<string> _userRoleIds;    //用户角色

        public List<Module> Modules
        {
            get { return Repository.GetAll().ToList(); }
        }
 
    }
}
