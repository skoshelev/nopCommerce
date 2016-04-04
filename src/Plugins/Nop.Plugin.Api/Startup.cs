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
using Nop.Plugin.Api.Swagger;
using Owin;
using Swashbuckle.Application;

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

            config.Routes.MapHttpRoute(
               name: "categories",
               routeTemplate: "api/categories",
               defaults: new { controller = "Categories", action = "GetCategories" });

            config.Routes.MapHttpRoute(
                name: "categoriesCount",
                routeTemplate: "api/categories/count",
                defaults: new { controller = "Categories", action = "GetCategoriesCount" });

            config.Routes.MapHttpRoute(
                name: "categoryById",
                routeTemplate: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "GetCategoryById" });

            config.Routes.MapHttpRoute(
              name: "products",
              routeTemplate: "api/products",
              defaults: new { controller = "Products", action = "GetProducts" });

            config.Routes.MapHttpRoute(
                name: "productsCount",
                routeTemplate: "api/products/count",
                defaults: new { controller = "Products", action = "GetProductsCount" });

            config.Routes.MapHttpRoute(
                name: "productById",
                routeTemplate: "api/products/{id}",
                defaults: new { controller = "Products", action = "GetProductById" });

            config.Routes.MapHttpRoute(
             name: "orders",
             routeTemplate: "api/orders",
             defaults: new { controller = "Orders", action = "GetOrders" });

            config.Routes.MapHttpRoute(
                name: "ordersCount",
                routeTemplate: "api/orders/count",
                defaults: new { controller = "Orders", action = "GetOrdersCount" });

            config.Routes.MapHttpRoute(
                name: "ordersByCustomerId",
                routeTemplate: "api/orders/customer/{customer_id}",
                defaults: new { controller = "Orders", action = "GetOrdersByCustomerId" });

            config.Routes.MapHttpRoute(
                name: "orderById",
                routeTemplate: "api/orders/{id}",
                defaults: new { controller = "Orders", action = "GetOrderById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            // The default route templates for the Swagger docs and swagger-ui are "swagger/docs/{apiVersion}" and "swagger/ui/index#/{assetPath}" respectively.
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "RESTful API documentation");
                    c.IncludeXmlComments(string.Format(@"{0}\Plugins\Nop.Plugin.Api\Nop.Plugin.Api.XML", AppDomain.CurrentDomain.BaseDirectory));
                    // We need this filter to exclude some of the API endpoints from the documentation i.e /OAuth/Authorize endpoint
                    c.DocumentFilter<ExcludeEnpointsDocumentFilter>();
                    c.OperationFilter<RemovePrefixesOperationFilter>();
                    c.OperationFilter<ChangeParameterTypeOperationFilter>();
                })
                .EnableSwaggerUi();

            app.UseWebApi(config);
            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(EngineContext.Current.ContainerManager.Container);
        }
    }
}

