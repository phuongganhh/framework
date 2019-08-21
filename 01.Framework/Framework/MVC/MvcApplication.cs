using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Framework.MVC
{
    public class MvcApplication : HttpApplication
    {
        protected virtual void Application_Start()
        {

        }
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_Authorizerequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_AcquireRequestState(object sender, EventArgs e)
        {
            
        }

    }
}
