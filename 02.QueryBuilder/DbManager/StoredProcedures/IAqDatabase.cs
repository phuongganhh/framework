using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace DbManager
{
    public interface IAqDatabase
    {
        string ConnectionStringName { get; }
        int ExecuteNonQuery(string cmdText, List<IDataParameter> paramItems = null);
        
        void ReadAsDataReader(string cmdText, Action<IDataReader> action, List<IDataParameter> paramItems = null);
        DataTable ReadAsDataTable(string cmdText, List<IDataParameter> paramItems = null);
        DataRow ReadAsDataRow(string cmdText, List<IDataParameter> paramItems = null);
     
        void CreateParameterWithAction(Action<DbCommand> cmd);
        IDataParameter CreateParameter(string name = null, object value = null, ParameterDirection direction = ParameterDirection.Input);
        IDataParameter CreateOutputParameter(string name, DbType dbType);
    }
}
