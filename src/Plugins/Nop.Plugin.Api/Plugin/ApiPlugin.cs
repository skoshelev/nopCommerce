using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Data;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Plugin
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        private const string ControllersNamespace = "Nop.Plugin.Payments.PayPalDirect.Controllers";

        private readonly ApiObjectContext _objectContext;

        public ApiPlugin(ApiObjectContext objectContext)
        {
            _objectContext = objectContext;
        }

        public override void Install()
        {
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret", "Client Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl", "Callback Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.AddNew", "Add New Client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Created", "Created");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Deleted", "Deleted");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name", "Name is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId", "Client Id is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret", "Client Secret is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl", "Callback Url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.BackToList", "Back To List");

            base.Install();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Api");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.IsActive");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.AddNew");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Created");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Deleted");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.BackToList");

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
            routeValues = new RouteValueDictionary { { "Namespaces", ControllersNamespace }, { "area", null } };
        }
    }
}
