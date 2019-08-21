using Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrainingMVC.Models;

namespace TrainingMVC.Controllers
{
    public class EmployeeController : FWController
    {
        public IHttpActionResult Get([FromUri] EmployeeGetListWithEFAction ActionCmd)
        {
            return Ok(ActionCmd.Execute(CurrentObjectContext));
        }
    }
}