using System;
using System.Collections.Generic;
using System.Web;
using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product, string fields = null)
        {
            var functions = new List<Func<IMappingExpression<Product, ProductDto>, IMappingExpression<Product, ProductDto>>>();

            functions.Add(map => map.IgnoreAllNonExisting());
            functions.Add(map => map.ForMember(x => x.FullDescription, y => y.MapFrom(src => HttpUtility.HtmlEncode(src.FullDescription))));
            functions.Add(map => map.MapOnly(fields, product));

            MappingEngine engine = AutoMapperHelper.CreateMapAndGetMappingEngine(fields, product, functions);

            return engine.Map<Product, ProductDto>(product);
        }
    }
}