using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Nop.Plugin.Api.ModelBinders
{
    // The idea comes from this article http://can-we-code-it.blogspot.co.uk/2015/04/handling-put-content-of-any-mime-type.html
    // but instead of using streams I am using the properties of the request.
    public class ParametersModelBinder<T> : IModelBinder where T : class, new()
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // MS_QueryNameValuePairs contains key value pair representation of the query parameters passed to in the request.
            if (actionContext.Request.Properties.ContainsKey("MS_QueryNameValuePairs"))
            {
                bindingContext.Model =
                    ((ICollection<KeyValuePair<string, string>>)
                        actionContext.Request.Properties["MS_QueryNameValuePairs"]).ToObject<T>();
            }
            else
            {
                bindingContext.Model = new T();
            }

            return true;
        }
    }

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
                    currentProperty.SetValue(someObject, Convert.ChangeType(item.Value, currentProperty.PropertyType), null);
                }
            }

            return someObject;
        }
    }
}