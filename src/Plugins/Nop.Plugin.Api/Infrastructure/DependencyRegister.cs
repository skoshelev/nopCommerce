using Autofac;
using Autofac.Core;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Api.Data;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Api.Infrastructure
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<ApiObjectContext>(builder, PluginNames.ObjectContextName);

            builder.RegisterType<EfRepository<Client>>()
               .As<IRepository<Client>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(PluginNames.ObjectContextName))
               .InstancePerLifetimeScope();

            CreateModelMappings();

            RegisterPluginServices(builder);
        }

        private void CreateModelMappings()
        {
            Mapper.CreateMap<ApiSettings, ConfigurationModel>();
            Mapper.CreateMap<ConfigurationModel, ApiSettings>();

            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel, Client>();
        }

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
        }

        public int Order { get; }
    }
}