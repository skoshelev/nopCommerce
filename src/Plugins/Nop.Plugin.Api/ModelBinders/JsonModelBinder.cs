using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.ModelBinders
{
    public class JsonModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // TODO: use await
            var requestPayload = actionContext.Request.Content.ReadAsStringAsync();

            if (requestPayload != null)
            {
                if (!string.IsNullOrEmpty(requestPayload.Result))
                {
                    Dictionary<string, object> result =
                        (Dictionary<string, object>)JsonHelper.Deserialize(requestPayload.Result);

                    bindingContext.Model = result;

                    return true;
                }
            }

            return false;
        }
    }
}