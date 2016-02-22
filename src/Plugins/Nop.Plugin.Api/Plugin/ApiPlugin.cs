using Nop.Core.Plugins;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Plugin
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
           
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
    }
}
