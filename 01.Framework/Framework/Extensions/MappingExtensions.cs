using Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Dynamic;
using System.Globalization;
using Framework.Common;
using AutoMapper;

namespace Framework.Extensions
{
    public static class MappingExtensions
    {
        public static List<dynamic> vnMappingToListDynamic(this DataTable dt)
        {
            List<dynamic> items = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                var dict = (IDictionary<string, object>)dyn;
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                items.Add(dyn);
            }
            return items;
        }

        public static dynamic vnMappingToDynamic(this IDataReader reader)
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


        public static TResult Map<TSource,TResult>(this TSource source)
        {
            return new MapperConfiguration(c =>
            {
                c.CreateMap<TSource, TResult>();
            }).CreateMapper().Map<TSource, TResult>(source);
        }
    }

}
