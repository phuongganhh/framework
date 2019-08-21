using Framework.Caching;
using Framework.Common;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace DbManager
{
    public static class DapperManager
    {
        private static SqlConnection conn { get; set; }
        private static string _connectionString { get; set; }
        private static string ConnectionString
        {
            get
            {
                if(_connectionString == null)
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                }
                return _connectionString;
            }
        }
        public static void Initial()
        {
            if(conn == null)
            {
                conn = new SqlConnection(ConnectionString);
            }
        }
        private static dynamic vnMappingToDynamic(this IDataReader reader)
        {
            dynamic item = new ExpandoObject();
            var dict = (IDictionary<string, object>)item;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i);
                dict.Add(reader.GetName(i), val.vnIsNull() ? null : val);
            }
            return item;
        }
        private static void MapParameter(this SqlParameterCollection param, Dictionary<string, Object> dic)
        {
            Parallel.ForEach(dic, entry => {
                param.AddWithValue(entry.Key, dic[entry.Key] ?? DBNull.Value);
            });
        }
        private static string ConvertToSqlServer(this SqlResult query)
        {
            return query.Sql;
        }
        private static void OpenConnection()
        {
            if (conn == null) return;
            if(conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }
        private static void CloseConnection()
        {
            if (conn == null) return;
            if(conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        public static List<T> Get<T>(this Query query) where T : class, new()
        {
            var items = new List<T>();
            DataTable data = new DataTable();
            OpenConnection();
            using (var cmd = conn.CreateCommand())
            {
                var sql = QueryHelper.CreateQueryFactory(query).Compiler.Compile(query);
                cmd.CommandText = sql.ConvertToSqlServer();
                MapParameter(cmd.Parameters, sql.NamedBindings);
                using (var reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (typeof(List<T>) == typeof(List<dynamic>))
                        {
                            items.Add(reader.vnMappingToDynamic());
                        }
                        else
                        {
                            items.Add(reader.CreateRecord<T>());
                        }

                    }
                    reader.Close();
                }
            }
            CloseConnection();
            return items;
        }
        private static T CreateRecord<T>(this IDataRecord record, T myClass = null) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();

            if (myClass == null)
                myClass = new T();
            for (int i = 0; i < record.FieldCount; i++)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.Name == record.GetName(i) && record.GetValue(i) != DBNull.Value)
                    {
                        propertyInfo.SetValue(myClass, Convert.ChangeType(record.GetValue(i), record.GetFieldType(i)), null);
                        break;
                    }
                }
            }
            return myClass;
        }
        private static IEnumerable<PropertyInfo> GetPropertiesWithCache(this Type type, string key = null, int CacheTimeByMinute = 3)
        {
            if (key == null)
            {
                key = type.FullName;
            }
            return MemoryCacheManager.Instance.GetOrSet(key, () => type.GetProperties(), CacheTimeByMinute);
        }
        private static List<string> GetParameter(string sql)
        {
            var ar = sql.Split(' ');
            var l = new List<string>();
            foreach (var item in ar)
            {
                if (item.IndexOf('@') > -1)
                {
                    l.Add(item);
                }
            }
            return l;
        }
        public static int Execute(this SqlResult query)
        {
            OpenConnection();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = query.ConvertToSqlServer();
                cmd.Parameters.MapParameter(query.NamedBindings);
                var reader = cmd.ExecuteNonQuery();
                CloseConnection();
                return Convert.ToInt32(reader);
            }
        }

    }
}
