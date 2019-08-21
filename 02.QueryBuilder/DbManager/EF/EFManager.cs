using DbManager.DbManager.EF;
using Entity.Training;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManager.EF
{
    public class EFManager : DbContext
    {
        public EFManager() : base("name=connectionString")
        {
            Database.SetInitializer<EFManager>(new CreateDatabaseIfNotExists<EFManager>());
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
