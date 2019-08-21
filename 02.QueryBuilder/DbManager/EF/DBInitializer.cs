using DbManager.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManager.DbManager.EF
{
    public class DBInitializer : CreateDatabaseIfNotExists<EFManager>
    {
        protected override void Seed(EFManager context)
        {
            base.Seed(context);
        }

    }
}
