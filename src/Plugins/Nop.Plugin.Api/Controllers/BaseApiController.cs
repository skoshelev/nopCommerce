using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;

namespace Nop.Plugin.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public BaseApiController(IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        protected IHttpActionResult Error()
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                if (errors.ContainsKey(item.Key))
                {
                    errors[item.Key].AddRange(errorMessages);
                }
                else
                {
                    errors.Add(item.Key, errorMessages.ToList());
                }
            }

            var errorsRootObject = new ErrorsRootObject()
            {
                Errors = errors
            };

            var errorsJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson);
        }
    }
}