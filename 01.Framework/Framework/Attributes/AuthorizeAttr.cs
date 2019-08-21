using Framework.Common;
using Framework.Controllers;
using Framework.Interfaces;
using Framework.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Framework.Attributes
{
    public class AuthorizeAttr : ActionFilterAttribute
    {
        private static readonly Type allowAnonymousAttr = typeof(AllowAnonymusAttr);

        private bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (!Enumerable.Any<AllowAnonymusAttr>((IEnumerable<AllowAnonymusAttr>)actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymusAttr>()))
                return Enumerable.Any<AllowAnonymusAttr>((IEnumerable<AllowAnonymusAttr>)actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymusAttr>());
            else
                return true;
        }

        private string TokenKey
        {
            get
            {
                return "access-token";
            }
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (this.SkipAuthorization(actionContext))
            {
                return;
            }

            void UnAuthorize()
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.OK,
                    new Result
                    {
                        Code = (int)HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        Message = "You are not logged in"
                    }
                );
                return;
            }
            var access_token = actionContext.Request.Headers.FirstOrDefault(x => x.Key == this.TokenKey);
            if(access_token.Value == null)
            {
                UnAuthorize();
                return;
            }
            else
            {
                var userId = access_token.Value.FirstOrDefault();
                if(userId == null)
                {
                    UnAuthorize();
                    return;
                }
                else
                {
                    this._controller = actionContext.ControllerContext.Controller as FWController;
                    this.SetIdentity(userId);
                }
            }

            base.OnActionExecuting(actionContext);
        }
        private FWController _controller { get; set; }
        private void SetIdentity(string userId)
        {
            this._controller.User =  new UserPrincipal(new UserContext {
                UserId = Convert.ToInt32(userId),
                Username = userId
            });
        }
    }
}
