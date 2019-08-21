using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Framework.User
{
    public class UserPrincipal : IUserPrincipal
    {
        public IIdentity Identity { get; private set; }
        public UserContext User { get; set; }
        public UserPrincipal(UserContext userId)
        {
            this.User = userId;
            this.Identity = new GenericIdentity(userId.Username);
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
