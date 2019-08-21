using Framework.Attributes;
using Framework.Controllers;
using System.Web.Http;
using TrainingMVC.Models;

namespace TrainingMVC.Controllers
{
    public class HomeController : FWController
    {
        [AuthorizeAttr]
        public IHttpActionResult Get([FromUri]EmployeeGetListAction ActionCmd)
        {
            return Ok(ActionCmd.Execute(CurrentObjectContext));
        }
    }
}