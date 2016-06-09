using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using Nop.Core;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.ModelBinders
{
    public class JsonModelBinder<T> : IModelBinder where T: class, new()
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public JsonModelBinder(IJsonHelper jsonHelper, ILocalizationService localizationService, IWorkContext workContext)
        {
            _jsonHelper = jsonHelper;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // TODO: use await
            // We need to get the language id before we made the actual request by askign for the result of the request payload,
            // because underneath nopCommerce uses the current customer which sets cookies and cookies can not be set if the 
            // request headers are send.
            int workingLanguageId = _workContext.WorkingLanguage.Id;
            var requestPayload = actionContext.Request.Content.ReadAsStringAsync();
            bool modelBined = false;

            if (requestPayload != null)
            {
                if (!string.IsNullOrEmpty(requestPayload.Result))
                {
                    Dictionary<string, object> result =
                        _jsonHelper.Deserialize(requestPayload.Result) as Dictionary<string, object>;

                    if (result != null && result.Count > 0)
                    {
                        // Needed so we can call the get the root name and validator type.
                        DtoAttribute dtoAttribute = typeof (T).GetCustomAttribute(typeof (DtoAttribute)) as DtoAttribute;

                        if (dtoAttribute == null)
                        {
                            return false;
                        }

                        Dictionary<string, object> propertyValuePaires = (Dictionary<string, object>) result[dtoAttribute.RootProperty];

                        var errors = ValidateValueTypeConvertion<T>(propertyValuePaires, workingLanguageId);

                        if (errors.Count == 0)
                        {
                            Delta<T> delta = new Delta<T>(propertyValuePaires);

                            Type validatorType = dtoAttribute.ValidatorType;

                            // We need to pass the http method because there are some differences between the validation rules for post and put
                            // We need to pass the propertyValuePaires from the passed json because there are cases in which one field is required
                            // on post, but it is a valid case not to pass it when doing a put request.    
                            var validator = Activator.CreateInstance(validatorType,
                                                new object[] {actionContext.Request.Method.ToString(),
                                                              propertyValuePaires });

                            // We know that the validator will be AbstractValidator<T> which means it will have Validate method.
                            ValidationResult validationResult = validatorType.GetMethod("Validate", new [] {typeof(T)})
                                .Invoke(validator, new [] { delta.Dto }) as ValidationResult;

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
                                bindingContext.Model = delta;
                                modelBined = true;
                            }
                        }
                        else
                        {
                            foreach (var error in errors)
                            {
                                bindingContext.ModelState.AddModelError(error.Key, error.Value);
                            }
                        }
                    }
                    else
                    {
                        bindingContext.ModelState.AddModelError("json",
                            _localizationService.GetResource("Api.InvalidJsonFormat", workingLanguageId, false));
                    }
                }
                else
                {
                    bindingContext.ModelState.AddModelError("json",
                            _localizationService.GetResource("Api.NoJsonProvided", workingLanguageId, false));
                }
            }
            else
            {
                // If we get to this point there is no request payload.
                bindingContext.ModelState.AddModelError("request",
                    _localizationService.GetResource("Api.InvalidRequest", workingLanguageId, false));
            }

            return modelBined;
        }

        private Dictionary<string, string> ValidateValueTypeConvertion<T>(Dictionary<string, object> propertyValuePaires, int languageId)
        {
            var errors = new Dictionary<string, string>();
            
            // Validate if the property value pairs passed maches the type.
            var typeValidator = new TypeValidator<T>();
            
            if (!typeValidator.IsValid(propertyValuePaires))
            {
                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = string.Format(_localizationService.GetResource("Api.InvalidType", languageId, false), invalidProperty);

                    if (!errors.ContainsKey(key))
                    {
                        errors.Add(key, _localizationService.GetResource("Api.InvalidPropertyType", languageId, false));
                    }
                }
            }

            return errors;
        }
    }
}