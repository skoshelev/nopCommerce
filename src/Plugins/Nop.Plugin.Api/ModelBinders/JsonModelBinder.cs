using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.ModelBinders
{
    public class JsonModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly int FirstLanguageId;

        public JsonModelBinder(IJsonHelper jsonHelper, ILocalizationService localizationService, ILanguageService languageService)
        {
            _jsonHelper = jsonHelper;
            _localizationService = localizationService;
            _languageService = languageService;

            // Languages are ordered by display order so the first language will be with the smallest display order.
            Language firstLanguage = _languageService.GetAllLanguages().FirstOrDefault();

            if (firstLanguage != null)
            {
                FirstLanguageId = firstLanguage.Id;
            }
            else
            {
                FirstLanguageId = 0;
            }
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // TODO: use await
            bool modelBined = false;

            Dictionary<string, object> result;
            DtoAttribute dtoAttribute;

            Validate(actionContext, bindingContext, out result, out dtoAttribute);

            if (bindingContext.ModelState.IsValid)
            {
                Dictionary<string, object> propertyValuePaires =
                    (Dictionary<string, object>)result[dtoAttribute.RootProperty];

                // You will have id parameter passed in the model binder only when you have put request.
                // because get and delete do not use the model binder.
                // Here we insert the id in the property value pairs to be validated by the dto validator in a later point.
                object routeDataId = GetRouteDataId(actionContext);

                if (routeDataId != null)
                {
                    // Here we insert the route data id in the value paires.
                    // If id is contained in the category json body the one from the route data is used instead.
                    InsertIdInTheValuePaires(propertyValuePaires, routeDataId);
                }

                // We need to call this method here so it will be certain that the routeDataId will be in the propertyValuePaires
                // when the request is PUT.
                ValidateValueTypeConvertion(bindingContext, propertyValuePaires);

                if (bindingContext.ModelState.IsValid)
                {
                    modelBined = BindModel(actionContext, bindingContext, propertyValuePaires, dtoAttribute.ValidatorType);
                }
            }

            return modelBined;
        }

        private void Validate(HttpActionContext actionContext, ModelBindingContext bindingContext,
            out Dictionary<string, object> result, out DtoAttribute dtoAttribute)
        {
            var requestPayload = actionContext.Request.Content.ReadAsStringAsync();

            // First we need to check if the request has payload. 
            // If the request does not have a payload it won't have any json provided.
            ValidateIfRequestHasPayload(bindingContext, requestPayload);

            // Now we need to check if there is any json provided in the request payload.
            CheckIfJsonIsProvided(bindingContext, requestPayload);

            // After we are sure that the request payload and json are provided we need to deserialize this json.
            result = DeserializeReqestPayload(bindingContext, requestPayload);

            // Next we have to validate the json format.
            ValidateJsonFormat(bindingContext, result);

            // Needed so we can call the get the root name and validator type.
            dtoAttribute = GetDtoAttribute(bindingContext);

            // After we have some dto attribute we need to validate it.
            ValidateDtoAttribute(bindingContext, dtoAttribute);

            // Now we need to validate the root property.
            ValidateRootProperty(bindingContext, result, dtoAttribute);
        }

        private object GetRouteDataId(HttpActionContext actionContext)
        {
            object routeDataId = null;

            if (actionContext.RequestContext.RouteData.Values.ContainsKey("id"))
            {
                routeDataId = actionContext.RequestContext.RouteData.Values["id"];
            }

            return routeDataId;
        }

        private void ValidateValueTypeConvertion(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires)
        {
            Dictionary<string, string> errors = GetValueTypeConvertionErrorsIfAny<T>(propertyValuePaires);

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    bindingContext.ModelState.AddModelError(error.Key, error.Value);
                }
            }
        }

        private void ValidateRootProperty(ModelBindingContext bindingContext, Dictionary<string, object> result, DtoAttribute dtoAttribute)
        {
            if (bindingContext.ModelState.IsValid)
            {
                bool isRootPropertyValid = result.ContainsKey(dtoAttribute.RootProperty);

                if (!isRootPropertyValid)
                {
                    bindingContext.ModelState.AddModelError("rootProperty", _localizationService.GetResource("Api.InvalidRootProperty", FirstLanguageId, false));
                }
            }
        }

        private void ValidateDtoAttribute(ModelBindingContext bindingContext, DtoAttribute dtoAttribute)
        {
            bool isDtoAttributePresent = dtoAttribute != null;

            // special validation for the dto.
            if (!isDtoAttributePresent && bindingContext.ModelState.IsValid)
            {
                // TODO: should this be present. The store owner shouldn't be able to get to this error if he does not create his own dtos.
                bindingContext.ModelState.AddModelError("dto", "invalid category dto");
            }
        }

        private DtoAttribute GetDtoAttribute(ModelBindingContext bindingContext)
        {
            DtoAttribute dtoAttribute = null;

            if (bindingContext.ModelState.IsValid)
            {
                dtoAttribute = typeof(T).GetCustomAttribute(typeof(DtoAttribute)) as DtoAttribute;
            }

            return dtoAttribute;
        }

        private void ValidateJsonFormat(ModelBindingContext bindingContext, Dictionary<string, object> result)
        {
            bool isJsonFormatValid = result != null && result.Count > 0;

            if (!isJsonFormatValid)
            {
                bindingContext.ModelState.AddModelError("json",
                    _localizationService.GetResource("Api.InvalidJsonFormat", FirstLanguageId, false));
            }
        }

        private Dictionary<string, object> DeserializeReqestPayload(ModelBindingContext bindingContext, Task<string> requestPayload)
        {
            Dictionary<string, object> result = null;

            // Here we check if validation has passed to this point.
            if (bindingContext.ModelState.IsValid)
            {
                result = _jsonHelper.Deserialize(requestPayload.Result) as Dictionary<string, object>;
            }

            return result;
        }

        private void CheckIfJsonIsProvided(ModelBindingContext bindingContext, Task<string> requestPayload)
        {
            bool hasJsonProvided = !string.IsNullOrEmpty(requestPayload.Result);

            if (!hasJsonProvided && bindingContext.ModelState.IsValid)
            {
                bindingContext.ModelState.AddModelError("json", _localizationService.GetResource("Api.NoJsonProvided", FirstLanguageId, false));
            }
        }

        private void ValidateIfRequestHasPayload(ModelBindingContext bindingContext, Task<string> requestPayload)
        {
            bool hasRequestPayload = requestPayload != null;

            if (!hasRequestPayload)
            {
                bindingContext.ModelState.AddModelError("request", _localizationService.GetResource("Api.InvalidRequest", FirstLanguageId, false));
            }
        }

        private bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext,
            Dictionary<string, object> propertyValuePaires, Type validatorType)
        {
            bool modelBined = false;

            Delta<T> delta = new Delta<T>(propertyValuePaires);

            // We need to pass the http method because there are some differences between the validation rules for post and put
            // We need to pass the propertyValuePaires from the passed json because there are cases in which one field is required
            // on post, but it is a valid case not to pass it when doing a put request.    
            var validator = Activator.CreateInstance(validatorType,
                new object[]
                {
                    actionContext.Request.Method.ToString(),
                    propertyValuePaires
                });

            // We know that the validator will be AbstractValidator<T> which means it will have Validate method.
            ValidationResult validationResult = validatorType.GetMethod("Validate", new[] { typeof(T) })
                .Invoke(validator, new[] { delta.Dto }) as ValidationResult;

            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    bindingContext.ModelState.AddModelError(validationFailure.PropertyName,
                        validationFailure.ErrorMessage);
                }
            }
            else
            {
                HandleAttributeInvokers(delta.Dto, bindingContext);

                bindingContext.Model = delta;
                modelBined = true;
            }

            return modelBined;
        }

        private void HandleAttributeInvokers(T dto, ModelBindingContext bindingContext)
        {
            var dtoProperties = dto.GetType().GetProperties();

            foreach (var property in dtoProperties)
            {
                // Check property type
                BaseAttributeInvoker invokerAttribute = property.PropertyType.GetCustomAttribute(typeof (BaseAttributeInvoker)) as BaseAttributeInvoker;

                // If not on property type, check the property itself.
                if (invokerAttribute == null)
                {
                    invokerAttribute = property.GetCustomAttribute(typeof (BaseAttributeInvoker)) as BaseAttributeInvoker;
                }

                if (invokerAttribute != null)
                {
                    invokerAttribute.Invoke(property.GetValue(dto));
                    Dictionary<string, string> errors = invokerAttribute.GetErrors();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            bindingContext.ModelState.AddModelError(error.Key, error.Value);
                        }
                    }
                }
            }
        }

        private void InsertIdInTheValuePaires(Dictionary<string, object> propertyValuePaires, object requestId)
        {
            if (propertyValuePaires.ContainsKey("id"))
            {
                propertyValuePaires["id"] = requestId;
            }
            else
            {
                propertyValuePaires.Add("id", requestId);
            }
        }

        private Dictionary<string, string> GetValueTypeConvertionErrorsIfAny<T>(Dictionary<string, object> propertyValuePaires)
        {
            var errors = new Dictionary<string, string>();

            // Validate if the property value pairs passed maches the type.
            var typeValidator = new TypeValidator<T>();

            if (!typeValidator.IsValid(propertyValuePaires))
            {
                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = string.Format(_localizationService.GetResource("Api.InvalidType", FirstLanguageId, false), invalidProperty);

                    if (!errors.ContainsKey(key))
                    {
                        errors.Add(key, _localizationService.GetResource("Api.InvalidPropertyType", FirstLanguageId, false));
                    }
                }
            }

            return errors;
        }
    }
}