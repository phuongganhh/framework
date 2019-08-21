using Framework;
using Framework.Common;
using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingMVC.Models.JWT
{
    public class JWTGenerateAction : CommandBase<string>
    {
        public string Name { get; set; }
        protected override Result<string> ExecuteCore(ObjectContext context)
        {
            return Success(this.Name.GenerateJWT());
        }
    }
}