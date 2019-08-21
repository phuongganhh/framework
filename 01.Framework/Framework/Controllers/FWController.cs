using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Framework.Controllers
{
    public class FWController : ApiController
    {
        public FWController() : base()
        {
            this.CreateObjectContext();
        }
        protected virtual void CreateObjectContext()
        {
            CurrentObjectContext = ObjectContext.CreateContext(this);
        }
        protected IHttpActionResult JsonExpando(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return Ok(obj);
        }
        public ObjectContext CurrentObjectContext { get; internal set; }
    }
}
