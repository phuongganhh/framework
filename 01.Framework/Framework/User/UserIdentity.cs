using Framework.Interfaces;
using Framework.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class UserIdentity : IIdentity
    {
        public UserContext User { get; set; }
        public string Name { get
            {
                return this.User.Username;
            }
        }
        public UserIdentity(UserContext user)
        {
            this.User = user;
        }
        public string AuthenticationType
        {
            get
            {
                return "Identity";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.User.UserId != null;
            }
        }
    }
}
