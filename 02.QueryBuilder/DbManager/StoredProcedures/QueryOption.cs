using System;
using System.Collections.Generic;
using System.Data;

namespace DbManager
{
    public class QueryOption : ICloneable
    {
        public QueryOption()
        {
            ConnectionName = "";
            Parameters = null;
            DataHandler = null;
            ListParameters = null;
        }

        /// <summary>
        /// Tên connection để xác định lấy ra chuối connection string.
        /// Mặc định: null => lấy chuối connection mặc định.
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Đối tượng được dùng làm tham số cho stored procedure. Mỗi tham số
        /// của stored procedure là một property của đối tượng này. Nếu stored
        /// procedure không có tham số thì không cần gán.
        /// </summary>
        public object Parameters { get; set; }

        /// <summary>
        /// Action chèn vào lúc đọc data từ database lên. Tham số của Action là
        /// IDataReader và sử dụng để xử lý làm một công việc khác (lấy ra để
        /// tạo file excel hoặc pdf chẳng hạn). Mặc định: null
        /// </summary>
        public Action<IDataReader> DataHandler { get; set; }

        public IEnumerable<object> ListParameters { get; set; }

        public object Clone()
        {
            var obj = new QueryOption();
            obj.ConnectionName = this.ConnectionName;
            obj.Parameters = this.Parameters;
            obj.DataHandler = this.DataHandler;
            obj.ListParameters = this.ListParameters;
            return obj;
        }
    }

    public class QueryOption<T> : QueryOption
    {
        public Func<IDataReader, T> DataSetHandler { get; set; }
    }
}
