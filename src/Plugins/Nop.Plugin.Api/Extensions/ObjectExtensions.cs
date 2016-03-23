using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Plugin.Api.Extensions
{
    public static class ObjectExtensions
    {
        public static T ToObject<T>(this ICollection<KeyValuePair<string, string>> source)
            where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, string> item in source)
            {
                var itemKey = item.Key.Replace("_", string.Empty);
                var currentProperty = someObjectType.GetProperty(itemKey,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (currentProperty != null)
                {
                    currentProperty.SetValue(someObject, To(item.Value, currentProperty.PropertyType), null);
                }
            }

            return someObject;
        }

        private static object To(string value, Type type)
        {
            if (type == typeof(DateTime?))
            {
                return value.ToDateTimeNullable();
            }
            else if (type == typeof(int))
            {
                return value.ToInt();
            }
            else if (type == typeof(List<int>))
            {
                return value.ToListOfInts();
            }
            else if(type == typeof(bool?))
            {
                // Because currently status is the only boolean and we need to accept published and unpublished statuses.
                return value.ToStatus();
            }

            // It should be the last resort, because it is not exception safe.
            return Convert.ChangeType(value, type);
        }
    }
}