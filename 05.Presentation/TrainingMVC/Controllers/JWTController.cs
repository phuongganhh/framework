using Framework.Controllers;
using System.Web.Http;
using TrainingMVC.Models.JWT;

namespace TrainingMVC.Controllers
{
    public class JWTController : FWController
    {
        public IHttpActionResult Get([FromUri] JWTGenerateAction ActionCmd)
        {
            return Ok(ActionCmd.Execute(CurrentObjectContext));
        }
    }
}