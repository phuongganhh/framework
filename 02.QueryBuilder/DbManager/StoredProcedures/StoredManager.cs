using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using Framework.Extensions;

namespace DbManager
{
    public static class StoredManager
    {
        #region fields and static constructor

        static Dictionary<Type, DbType> typeMap;

        public static IUnitOfWork UnitOfWork { get; set; }
        public static IAqDatabase DefaultDb { get; set; }

        static StoredManager()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
            UnitOfWork = ServiceLocator.Current.GetInstance<IUnitOfWork>();
            DefaultDb = UnitOfWork.GetDatabase();
        }

        #endregion

        /// <summary>
        /// private QueryOption extension
        /// </summary>
        public static IAqDatabase GetDb(this QueryOption opt)
        {
            return UnitOfWork.GetDatabase(opt.ConnectionName);
        }

        #region for executing store or sql queries

        /// <summary>
        /// Gọi và thực hiện một stored procedure. Tham số input/output sẽ được lấy/gán
        /// vào tham số QueryOption.
        /// </summary>
        /// <param name="cmdText">Tên stored</param>
        /// <param name="option">Option</param>
        /// <returns>Số dòng bị thay đổi sau khi thực hiện stored procedure</returns>
        public static int Execute(string cmdText, QueryOption option)
        {
            if (option.ListParameters != null)
            {
                var clone = option.Clone() as QueryOption;
                clone.ListParameters = null;
                var count = 0;
                foreach (var p in option.ListParameters)
                {
                    clone.Parameters = p;
                    var c = StoredManager.Execute(cmdText, clone);
                    if (c > 0)
                    {
                        count += c;
                    }
                }
                return count;
            }
            else
            {
                var db = option.GetDb();
                IList<string> output;
                var parameters = createListParameters(cmdText, option, db, out output);
                int rowCount = db.ExecuteNonQuery(cmdText, parameters);
                handleOutput(parameters, output, option.Parameters);
                return rowCount;
            }
        }

        /// <summary>
        /// Gọi và thực hiện một stored procedure. Tham số input/output sẽ được lấy/gán
        /// vào tham số QueryOption. Kết quả trả về là action với tham số IDataReader, ta
        /// sẽ lấy dữ liệu và xử lý trong action này. Chú ý không gán reader ra ngoài rồi sử
        /// dụng vì sau khi hàm này được gọi thì reader sẽ bị Close, tất cả phải được làm
        /// trong action. Bạn cũng không cần close connection sau khi sử dụng.
        /// </summary>
        /// <param name="cmdText">Tên stored</param>
        /// <param name="option">Option</param>
        /// <param name="action">Action dùng để lấy và xử lý dữ liệu được select lên</param>
        private static void ExecuteAsReader(string cmdText, QueryOption option, Action<IDataReader> action)
        {
            if (option.ListParameters != null)
            {
                var clone = option.Clone() as QueryOption;
                clone.ListParameters = null;
                foreach (var p in option.ListParameters)
                {
                    clone.Parameters = p;
                    StoredManager.ExecuteAsReader(cmdText, clone, action);
                }
            }
            else
            {
                var db = option.GetDb();
                IList<string> output;
                var parameters = createListParameters(cmdText, option, db, out output);
                db.ReadAsDataReader(cmdText, action, parameters);
                handleOutput(parameters, output, option.Parameters);
            }
        }

        /// <summary>
        /// Gọi và thực hiện một stored procedure. Tham số input/output sẽ được lấy/gán
        /// vào tham số QueryOption. Kết quả trả về một DataTable.
        /// </summary>
        /// <param name="cmdText">Tên stored</param>
        /// <param name="option">Option</param>
        /// <returns>Đối tượng datatable chứa dữ liệu trả về của stored</returns>
        private static DataTable QueryAsDataTable(string cmdText, QueryOption option)
        {
            DataTable data = new DataTable();
            ExecuteAsReader(cmdText, option, new Action<IDataReader>(reader =>
            {
                DataTable schemaTable = reader.GetSchemaTable();

                foreach (DataRow row in schemaTable.Rows)
                {
                    string colName = row.Field<string>("ColumnName");
                    Type t = row.Field<Type>("DataType");
                    data.Columns.Add(colName, t);
                }
                if (option.DataHandler == null)
                {
                    while (reader.Read())
                    {
                        var newRow = data.Rows.Add();
                        foreach (DataColumn col in data.Columns)
                        {
                            newRow[col.ColumnName] = reader[col.ColumnName];
                        }
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        option.DataHandler(reader);

                        var newRow = data.Rows.Add();
                        foreach (DataColumn col in data.Columns)
                        {
                            newRow[col.ColumnName] = reader[col.ColumnName];
                        }
                    }
                }
            }));
            return data;
        }

        public static List<T> QuerySet<T>(string cmdText, QueryOption<T> option) where T : class, new()
        {
            var items = new List<T>();

            ExecuteAsReader(cmdText, option, new Action<IDataReader>(reader =>
            {
                while (reader.Read())
                {
                    if (option.DataSetHandler != null)
                    {
                        items.Add(option.DataSetHandler(reader));
                    }
                    else
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
                }
            }));

            return items;
        }

        private static T CreateRecord<T>(this IDataRecord record, T myClass = null) where T :class, new()
        {
            var propertyInfos = typeof(T).GetProperties();

            if(myClass == null)
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

        /// <summary>
        /// Gọi và thực hiện một stored procedure. Tham số input/output sẽ được lấy/gán
        /// vào tham số QueryOption. Kết quả trả về dòng đâu tiên tìm thấy.
        /// </summary>
        /// <param name="cmdText">Tên stored</param>
        /// <param name="option">Option</param>
        /// <returns>Đối tượng DataRow chứa dữ liệu của dòng đầu tiên tìm thấy</returns>
        public static DataRow QueryAsDataRow(string cmdText, QueryOption option)
        {
            DataTable dt = QueryAsDataTable(cmdText, option);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }



        #endregion

        #region private handlers

        private static void handleOutput(IList<IDataParameter> parameters, IList<string> outputParameters, object obj)
        {
            PropertyInfo[] properties;
            if (obj == null)
            {
                properties = new PropertyInfo[] { };
            }
            else
            {
                properties = obj.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            var outputProperties = properties.Where(x => outputParameters.Contains(x.Name.ToLower())).ToList();
            foreach (var prop in outputProperties)
            {
                IDataParameter p = parameters.FirstOrDefault(x => x.ParameterName.ToLower() == prop.Name.ToLower());
                if (p != null)
                {
                    if (p.Value.GetType() == typeof(DBNull))
                    {
                        prop.SetValue(obj, Activator.CreateInstance(prop.PropertyType));
                    }
                    else
                    {
                        prop.SetValue(obj, p.Value);
                    }
                }
            }
        }
        private static List<IDataParameter> CreateListParameter(this IAqDatabase db, object obj = null)
        {
            if (obj == null)
                return new List<IDataParameter>();
            var props = obj.GetType().GetProperties();
            var paraCollection = new List<IDataParameter>();
            db.CreateParameterWithAction(cmd =>
            {
                foreach (var prop in props)
                {
                    var para = cmd.CreateParameter();
                    para.ParameterName = prop.Name;
                    para.Value = prop.GetValue(obj) ?? DBNull.Value;
                    paraCollection.Add(para);
                }

            });
            return paraCollection;
        }
        private static List<IDataParameter> createListParameters(string cmdText, QueryOption option, IAqDatabase db, out IList<string> output)
        {
            List<string> listParameters;
            output = new List<string>();
            if (option.Parameters == null)
            {
                listParameters = new List<string>();
            }
            else
            {
                if (!cmdText.Contains(' '))
                {
                    listParameters = getParametersFromSp(cmdText, db, output);
                }
                else
                {
                    listParameters = getParametersFromQuery(cmdText).ToList();
                }
            }
            listParameters = listParameters.Select(x => x.ToLower()).ToList();

            PropertyInfo[] properties;
            if (option.Parameters == null)
            {
                properties = new PropertyInfo[] { };
            }
            else
            {
                properties = option.Parameters.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }

            // Kiem tra cac propeties trong object co bi trung ten khong
            // vi sql khong phan biet hoa thuong
            var duplicates = properties.GroupBy(x => x.Name.ToLower())
                                       .Where(g => g.Count() > 1)
                                       .ToList();
            if (duplicates.Count > 0)
            {
                string msg = "Duplicated properties: " + string.Join(", ", duplicates);
                throw new DuplicateNameException(msg);
            }

            var p = db.CreateListParameter();
            foreach (var property in properties)
            {
                string name = property.Name.ToLower();
                object value = property.GetValue(option.Parameters);
                if (listParameters.Contains(name))
                {
                    if (output.Contains(name))
                    {
                        p.Add(db.CreateOutputParameter(name, typeMap[property.PropertyType]));
                    }
                    else
                    {
                        p.Add(db.CreateParameter(name, value));
                    }
                }
            }
            return p;
        }
        static string reSqlParameter = @"(?<=^([^']|'[^']*')*)\@(\w+)";
        private static IEnumerable<string> getParametersFromQuery(string sqlQuery)
        {
            Regex regex = new Regex(reSqlParameter, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = regex.Matches(sqlQuery);
            foreach (Match match in matches)
            {
                yield return match.Groups[2].Value;
            }
        }
        //	@procedure_schema	sysname = null,

        private static List<string> getParametersFromSp(string cmdText, IAqDatabase db, IList<string> output)
        {
            var p = db.CreateListParameter();
            p.Add(db.CreateParameter("procedure_name", cmdText));
            p.Add(db.CreateParameter("procedure_schema", "dbo"));
            var dt = db.ReadAsDataTable("sp_procedure_params_100_managed", p);

            var listParameterName = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string name = row.Field<string>("PARAMETER_NAME").ToLower();
                int type = row.Field<short>("PARAMETER_TYPE");

                name = name.Replace("@", "");
                if (type == 2)
                {
                    output.Add(name);
                    listParameterName.Add(name);
                }
                else if (type == 1)
                {
                    listParameterName.Add(name);
                }
            }
            return listParameterName;
        }

        #endregion

    }
}
