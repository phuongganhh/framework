using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DbManager
{
    public interface IAqRepository
    {

        int ExecuteNonQuery(string cmdText, List<IDataParameter> paramItems = null);
        void ReadAsDataReader(string cmdText, Action<IDataReader> action, List<IDataParameter> paramItems = null);
        DataTable ReadAsDataTable(string cmdText, List<IDataParameter> paramItems = null);

        T Get<T>(string cmdText, List<IDataParameter> paramItems = null) where T : new();
        List<T> GetList<T>(string cmdText, List<IDataParameter> paramItems = null) where T : new();
    }
}
