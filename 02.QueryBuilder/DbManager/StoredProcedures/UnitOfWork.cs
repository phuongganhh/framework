using System;
using System.Collections.Generic;
using System.Configuration;
using System.Transactions;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace DbManager
{
    public class UnitOfWork : IUnitOfWork
    {
        string defaultName;
        protected List<IAqDatabase> Databases { get; set; }
        public UnitOfWork()
        {
            Databases = new List<IAqDatabase>();
            //foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
            //{
            //    Databases.Add(new AqDatabase(css.Name));
            //}
            var dataConfiguration = ConfigurationManager.GetSection(DatabaseSettings.SectionName) as DatabaseSettings;
            defaultName = dataConfiguration.DefaultDatabase;
            Databases.Add(new AqDatabase(defaultName));
        }

        public IAqDatabase GetDatabase(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = defaultName;
            }
            foreach (var item in Databases)
            {
                if(item.ConnectionStringName == name)
                {
                    return item;
                }
            }
            return null;
        }
        public void ExecuteTransaction(Action cmdTransaction, bool SetCompleted = false)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                cmdTransaction();
                if (SetCompleted == true)
                {
                    scope.Complete();
                }
            }
        }
        public void Commit()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                scope.Complete();
            }
        }
    }
}
