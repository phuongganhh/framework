using System;

namespace DbManager
{
    public interface IUnitOfWork
    {
        IAqDatabase GetDatabase(string name = null);
        void ExecuteTransaction(Action cmdTransaction, bool SetCompleted = false);
        void Commit();
    }
}
