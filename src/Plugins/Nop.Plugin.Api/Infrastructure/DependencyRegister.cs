using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Data;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Api.Infrastructure
{
    public class DependencyRegister : IDependencyRegistrar
    {
		private const string ObjectContextName = "nop_object_context_web_api";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<ApiObjectContext>(builder, ObjectContextName);

            builder.RegisterType<EfRepository<Client>>()
               .As<IRepository<Client>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
               .InstancePerLifetimeScope();

            CreateModelMappings();

            RegisterPluginServices(builder);

            RegisterControllers(builder);
        }

        private void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterType<CustomersController>().InstancePerLifetimeScope();
        }

        private void CreateModelMappings()
        {
            Maps.CreateMap<ApiSettings, ConfigurationModel>();
            Maps.CreateMap<ConfigurationModel, ApiSettings>();

            Maps.CreateMap<Client, ClientModel>();
            Maps.CreateMap<ClientModel, Client>();

            Maps.CreateAddressMap();
            Maps.CreateShoppingCartItemMap();

            Maps.CreateCustomerToDTOMap();
            Maps.CreateCustomerToOrderCustomerDTOMap();
            Maps.CreateCustomerForShoppingCartItemMapFromCustomer();
        }

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<WebConfigMangerHelper>().As<IWebConfigMangerHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceApiService>().As<IStateProvinceApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryApiService>().As<ICountryApiService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationHelper>().As<IAuthorizationHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();
        }

        public int Order { get; }
    }
}