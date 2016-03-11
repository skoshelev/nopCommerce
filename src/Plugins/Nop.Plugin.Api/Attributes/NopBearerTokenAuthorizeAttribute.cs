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

            // Swagger client does not support BearerToken authentication.
            // That is why we don't check for Bearer token authentication but check only 2 things:
            // 1. The store owner explicitly has allowed Swagger to make requests to the API
            // 2. Check if the request really comes from Swagger documentation page. Since Swagger documentation page is located on /swagger/ui/index we simply check that the Refferer contains "swagger"
            if (settings.AllowRequestsFromSwagger && actionContext.Request.Headers.Referrer != null && actionContext.Request.Headers.Referrer.ToString().Contains("swagger"))
            {
                return true;
            }

            // Since we support only BearerToken authorization, we need to make sure that the request to the server is a BearerToken request.
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
            // By default nopCommerce uses Forms authentication so it will redirect to the Login page any unauthorised requests. 
            // But we don't want any unauthorised requests to the API endpoints to be redirected to the Login page.
            // We ensure this won't happen by suppressing the forms authentication redirect.
            // This way the client will just receive a message "Unauthorized request"
            HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}