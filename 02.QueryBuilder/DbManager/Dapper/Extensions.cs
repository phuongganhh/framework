using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbManager.Drapper
{
    public static class Extensions
    {
        private static void MapParameter(this SqlParameterCollection param, Dictionary<string, Object> dic)
        {
            foreach (KeyValuePair<string, Object> entry in dic)
            {
                param.AddWithValue(entry.Key, dic[entry.Key] ?? DBNull.Value);
            }
        }
    }
}
