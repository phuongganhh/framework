using Entity.Training;
using Framework;
using Framework.Interfaces;
using Repository.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingMVC.Models
{
    public class EmployeeGetListWithEFAction : CommandBase<List<Employee>>
    {
        private Result<List<Employee>> GetEmployee(ObjectContext context)
        {
            using(var cmd = new EmployeeGetListWithEFRepositor())
            {
                return cmd.Execute(context);
            }
        }
        protected override Result<List<Employee>> ExecuteCore(ObjectContext context)
        {
            return this.GetEmployee(context);
        }
    }
}