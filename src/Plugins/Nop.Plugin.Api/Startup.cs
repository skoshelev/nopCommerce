using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Owin.Middleware;
using Nop.Plugin.Api.Owin.OAuth.Providers;
using Owin;

namespace Nop.Plugin.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);

            app.UseStageMarker(PipelineStage.PostAuthenticate);

            ConfigureWebApi(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            // The token endpoint path activates the ValidateClientAuthentication method from the AuthorisationServerProvider.
            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AuthorizeEndpointPath = new PathString("/OAuth/Authorize"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Configurations.AccessTokenExpiration),
                Provider = new AuthorisationServerProvider(),
                AuthorizationCodeProvider = new AuthenticationTokenProvider(),
                RefreshTokenProvider = new RefreshTokenProvider(),
                ApplicationCanDisplayErrors = true
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);


            // Our own middleware that resets the current user set by the Forms authentication in case we have a Bearer token request
            app.Use(typeof(BearerTokenMiddleware));

            // This middleware should be called after the BearerTokenMiddleware
            app.Use(typeof(OAuthBearerAuthenticationMiddleware), app, new OAuthBearerAuthenticationOptions());            
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            config.Routes.MapHttpRoute(
                name: "authorizeApi",
                routeTemplate: "OAuth/Authorize",
                defaults: new { controller = "OAuth", action = "Authorize" });

            config.Routes.MapHttpRoute(
              name: "customers",
              routeTemplate: "api/customers",
              defaults: new { controller = "Customers", action = "GetCustomers" });

            config.Routes.MapHttpRoute(
                name: "customersCount",
                routeTemplate: "api/customers/count",
                defaults: new { controller = "Customers", action = "GetCustomersCount" });

            config.Routes.MapHttpRoute(
                name: "customerSearch",
                routeTemplate: "api/customers/search",
                defaults: new { controller = "Customers", action = "Search" });

            config.Routes.MapHttpRoute(
                name: "customerById",
                routeTemplate: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "GetCustomerById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            app.UseWebApi(config);
            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(EngineContext.Current.ContainerManager.Container);
        }
    }
}

