using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Plugin.Api.Extensions
{
    public class ObjectExtensions : IObjectExtensions
    {
        private readonly IStringExtensions _stringExtensions;

        public ObjectExtensions(IStringExtensions stringExtensions)
        {
            _stringExtensions = stringExtensions;
        }

        public T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
            where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            if (source != null)
            {
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
            }

            return someObject;
        }

        private object To(string value, Type type)
        {
            if (type == typeof(DateTime?))
            {
                return _stringExtensions.ToDateTimeNullable(value);
            }
            else if (type == typeof (int?))
            {
                return _stringExtensions.ToIntNullable(value);
            }
            else if (type == typeof(int))
            {
                return _stringExtensions.ToInt(value);
            }
            else if (type == typeof(List<int>))
            {
                return _stringExtensions.ToListOfInts(value);
            }
            else if(type == typeof(bool?))
            {
                // Because currently status is the only boolean and we need to accept published and unpublished statuses.
                return _stringExtensions.ToStatus(value);
            }

            // It should be the last resort, because it is not exception safe.
            return Convert.ChangeType(value, type);
        }
    }
}