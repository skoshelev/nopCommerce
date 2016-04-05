﻿using System.Collections.Generic;
using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Customers;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class Maps
    {
        public static IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            return Mapper.CreateMap<TSource, TDestination>().IgnoreAllNonExisting();
        }

        public static void CreateAddressMap()
        {
            Mapper.CreateMap<Address, AddressDto>()
               .IgnoreAllNonExisting()
               .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
               .ForMember(x => x.CountryName, y => y.MapFrom(src => src.Country.GetWithDefault(x => x, new Country()).Name))
               .ForMember(x => x.StateProvinceName, y => y.MapFrom(src => src.StateProvince.GetWithDefault(x => x, new StateProvince()).Name));
        }
        
        public static void CreateCustomerForShoppingCartItemMapFromCustomer()
        {
            Mapper.CreateMap<Customer, CustomerForShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress, y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress, y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.Addresses, y => y.MapFrom(src => src.Addresses.GetWithDefault(x => x, new List<Address>()).Select(address => address.ToDto())));
        }
        
        public static void CreateCustomerToDTOMap()
        {
            Mapper.CreateMap<Customer, CustomerDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress,
                    y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress,
                    y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.Addresses,
                    y =>
                        y.MapFrom(
                            src =>
                                src.Addresses.GetWithDefault(x => x, new List<Address>())
                                    .Select(address => address.ToDto())))
                .ForMember(x => x.ShoppingCartItems,
                    y =>
                        y.MapFrom(
                            src =>
                                src.ShoppingCartItems.GetWithDefault(x => x, new List<ShoppingCartItem>())
                                    .Select(item => item.ToDto())));
        }

        public static void CreateCustomerToOrderCustomerDTOMap()
        {
            Mapper.CreateMap<Customer, OrderCustomerDto>()
                .IgnoreAllNonExisting();
        }

        public static void CreateShoppingCartItemMap()
        {
            Mapper.CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Customer, y => y.MapFrom(src => src.Customer.GetWithDefault(x => x, new Customer()).ToCustomerForShoppingCartItemDto()))
                .ForMember(x => x.Product, y => y.MapFrom(src => src.Product.GetWithDefault(x => x, new Product()).ToDto(null)));
        }

        public static void CreateProductMap()
        {
            Mapper.CreateMap<Product, ProductDto>()
               .IgnoreAllNonExisting()
               .ForMember(x => x.FullDescription, y => y.MapFrom(src => HttpUtility.HtmlEncode(src.FullDescription)));
        }
    }
}