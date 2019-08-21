using Entity.Training;
using Framework;
using Framework.Interfaces;
using Repository.Training;
using System.Collections.Generic;

namespace TrainingMVC.Models
{
    public class EmployeeGetListAction : CommandBase<List<Employee>>
    {
        private Result<List<Employee>> GetEmployee(ObjectContext context)
        {
            using(var cmd = new EmployeeGetListRepository<Employee>())
            {
                return cmd.Execute(context);
            }
        }
        protected override Result<List<Employee>> ExecuteCore(ObjectContext context)
        {
            var user = context.GetUser;
            return this.GetEmployee(context);
        }
    }
}