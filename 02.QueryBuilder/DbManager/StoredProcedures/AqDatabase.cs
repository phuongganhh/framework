using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DbManager
{
    /// <summary>
    /// 
    /// </summary>
    public class AqDatabase : IAqDatabase
    {
        protected static DatabaseProviderFactory factory = new DatabaseProviderFactory();
        protected Database db;
        private string _connectionStringName;
        public string ConnectionStringName
        {
            get
            {
                return _connectionStringName;
            }
        }
        public AqDatabase()
            : this(null)
        {
        }
        public AqDatabase(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                db = factory.CreateDefault();
            }
            else
            {
                db = factory.Create(connectionStringName);
            }
        }

        public int ExecuteNonQuery(string cmdText, List<IDataParameter> paramItems = null)
        {
            cmdText = cmdText.Trim();
            DbCommand cmd;
            if (!cmdText.Contains(' '))///this is select sql
            {
                cmd = db.GetStoredProcCommand(cmdText);
            }
            else
            {
                cmd = db.GetSqlStringCommand(cmdText);
            }

            ///=======================
            cmd.Parameters.Clear();
            if (paramItems != null)
            {
                foreach (var para in paramItems)
                {
                    cmd.Parameters.Add(para);
                }
            }
            var result = db.ExecuteNonQuery(cmd);
            return result;
        }

        public void ReadAsDataReader(string cmdText, Action<IDataReader> action, List<IDataParameter> paramItems = null)
        {
            cmdText = cmdText.Trim();
            DbCommand cmd;
            if (!cmdText.Contains(' '))///this is select sql
            {
                cmd = db.GetStoredProcCommand(cmdText);
            }
            else
            {
                cmd = db.GetSqlStringCommand(cmdText);
            }

            ///=======================
            cmd.Parameters.Clear();
            if (paramItems != null)
            {
                foreach (var para in paramItems)
                {
                    cmd.Parameters.Add(para);
                }
            }
            using (var reader = db.ExecuteReader(cmd))
            {
                action(reader);
                reader.Close();
            }
        }
        public DataTable ReadAsDataTable(string cmdText, List<IDataParameter> paramItems = null)
        {
            var dataTable = new DataTable();
            ReadAsDataReader(cmdText, reader =>
            {
                dataTable.Load(reader);
            }, paramItems);
            return dataTable;
        }
        public DataRow ReadAsDataRow(string cmdText, List<IDataParameter> paramItems = null)
        {
            var dataTable = ReadAsDataTable(cmdText, paramItems);
            if (dataTable.Rows.Count == 0)
                return null;
            return dataTable.Rows[0];
        }
        

        public void CreateParameterWithAction(Action<DbCommand> action)
        {
            using (var cmd = db.GetSqlStringCommand("select 1"))
            {
                action(cmd);
            }
        }
        public IDataParameter CreateParameter(string name = null, object value = null,ParameterDirection direction = ParameterDirection.Input)
        {
            IDataParameter result = null;
            CreateParameterWithAction(cmd =>
            {
                result = cmd.CreateParameter();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    result.ParameterName = name;
                    result.Value = value ?? DBNull.Value;
                    result.Direction = direction;
                }
            });
            return result;
        }
        public IDataParameter CreateOutputParameter(string name, DbType dbType)
        {
            IDataParameter result = null;
            CreateParameterWithAction(cmd =>
            {
                result = cmd.CreateParameter();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    result.ParameterName = name;
                    result.Direction = ParameterDirection.Output;
                    result.DbType = dbType;

                    if (dbType == DbType.String)
                    {
                        var cast = result as IDbDataParameter;
                        if (cast != null)
                        {
                            cast.Size = 4000;
                        }
                    }
                }
            });
            return result;
        }
    }
}
