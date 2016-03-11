using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Attributes
{
    public class NopBearerTokenAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IAuthorizationHelper _authorizationHelper = EngineContext.Current.Resolve<IAuthorizationHelper>();

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            ApiSettings settings = EngineContext.Current.Resolve<ApiSettings>();

            if (actionContext.Request.Headers.Referrer.ToString().Contains("swagger") && settings.AllowRequestsFromSwagger)
            {
                return true;
            }

            // We need to make sure that the request to the server is not a regular request but only BearerToken requests
            // If we don't do this here, since the customer is already authenticated from the FormsAuthentication in Nop,  he 
            // will have access to this resource
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization == null || authorization.Scheme != "Bearer" || !settings.EnableApi || !_authorizationHelper.ClientExistsAndActive())
            {
                return false;
            }

            return base.IsAuthorized(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}