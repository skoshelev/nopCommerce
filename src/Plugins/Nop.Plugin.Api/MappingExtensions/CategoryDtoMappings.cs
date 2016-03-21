using System;
using System.Collections.Generic;
using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Categories;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CategoryDtoMappings
    {
        public static CategoryDto ToDto(this Category category, string fields = null)
        {
            var functions = new List<Func<IMappingExpression<Category, CategoryDto>, IMappingExpression<Category, CategoryDto>>>();

            functions.Add(x => x.IgnoreAllNonExisting());
            
            return Mapper.Map<Category, CategoryDto>(category);
        }
    }
}