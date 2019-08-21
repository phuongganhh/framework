using DbManager.EF;
using Entity.Training;
using Framework;
using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Training
{
    public class EmployeeGetListWithEFRepositor: CommandBase<List<Employee>>
    {
        private List<Employee> GetEmployee(ObjectContext context)
        {
            var db = new EFManager();
            return db.Employees.ToList();
        }
        protected override Result<List<Employee>> ExecuteCore(ObjectContext context)
        {
            return Success(this.GetEmployee(context));
        }
    }
}
