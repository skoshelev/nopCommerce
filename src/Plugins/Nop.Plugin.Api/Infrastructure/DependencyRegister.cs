using Autofac;
using AutoMapper;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.Infrastructure
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            CreateModelMappings();
        }

        private void CreateModelMappings()
        {
            Mapper.CreateMap<ApiSettings, ConfigurationModel>();
            Mapper.CreateMap<ConfigurationModel, ApiSettings>();
        }

        public int Order { get; }
    }
}