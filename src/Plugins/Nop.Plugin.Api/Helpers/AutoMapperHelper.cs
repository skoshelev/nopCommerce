using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Mappers;

namespace Nop.Plugin.Api.Helpers
{
    public class AutoMapperHelper
    {
        // The purpose of this dictionary is to avoid multiple map creations for repeating fields parameter.
        // i.e. If we ask only for the name, we should create only one map and that map shouldn't be recreated
        // if another call with the same fields is made.
        private static readonly Dictionary<string, Tuple<MappingEngine, ConfigurationStore>> EnginesAndConfigurationStores =
            new Dictionary<string, Tuple<MappingEngine, ConfigurationStore>>();

        private const string NoFieldsKey = "nofields";

        public static MappingEngine CreateMapAndGetMappingEngine<TSource, TDestination>(string fields, TSource entity,
            List<Func<IMappingExpression<TSource, TDestination>, IMappingExpression<TSource, TDestination>>> functions)
        {
            MappingEngine engine;
            // Here we fill the dictionary of mapping engines and configuration stores, so we can use different mapping engine 
            // for every variation of the fields parameter.
            // This is needed because every mapping engine caches only one mapping per object types (i.e. Category -> CategoryDto), 
            // which means that if I have Category -> CategoryDto created for one variation of the fields parameter, 
            // I can not have a second one (instead the first one will be used). 
            // src - http://stackoverflow.com/questions/1668962/using-the-instance-version-of-createmap-and-map-with-a-wcf-service/7380377
            string fieldsVariationKey = fields ?? NoFieldsKey;

            if (!EnginesAndConfigurationStores.ContainsKey(fieldsVariationKey))
            {
                var configurationStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
                engine = new MappingEngine(configurationStore);

                CreateMap(configurationStore, functions);

                EnginesAndConfigurationStores.Add(fieldsVariationKey,
                    new Tuple<MappingEngine, ConfigurationStore>(engine, configurationStore));
            }
            // In here we check if the current engine already has map between the source and destination types.
            // If it does not have such map we create one, otherwise we return the engine.
            // This is done so we can reuse the engine for multiple types, otherwise we will have different engines for each type,
            // which is waste of resources.
            else
            {
                Tuple<MappingEngine, ConfigurationStore> engineAndStore =
                    EnginesAndConfigurationStores[fieldsVariationKey];

                engine = engineAndStore.Item1;

                TypeMap map = engine.ConfigurationProvider.FindTypeMapFor(typeof(TSource), typeof(TDestination));

                if (map == null)
                {
                    ConfigurationStore configurationStore = engineAndStore.Item2;
                    CreateMap(configurationStore, functions);
                }
            }

            return engine;
        }

        private static IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(ConfigurationStore configurationStore,
          List<Func<IMappingExpression<TSource, TDestination>, IMappingExpression<TSource, TDestination>>> functions)
        {
            IMappingExpression<TSource, TDestination> mappingExpression = configurationStore.CreateMap<TSource, TDestination>();

            foreach (var function in functions)
            {
                function(mappingExpression);
            }

            return mappingExpression;
        }
    }
}