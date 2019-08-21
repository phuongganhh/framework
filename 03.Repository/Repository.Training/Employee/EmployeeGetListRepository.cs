using DbManager;
using Framework;
using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Training
{
    public class EmployeeGetListRepository<T> : CommandBase<List<T>> where T : class, new()
    {
        private List<T> GetEmployee(ObjectContext context)
        {
            return context.db.From("Employee").Get<T>();
        }
        protected override Result<List<T>> ExecuteCore(ObjectContext context)
        {
            return Success(this.GetEmployee(context));
        }
    }
}
