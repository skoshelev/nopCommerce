using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Api.MVC;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Plugin
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web API");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Api");
            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
           
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ApiAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", PluginNames.ControllersNamespace }, { "area", null } };
        }
    }
}
