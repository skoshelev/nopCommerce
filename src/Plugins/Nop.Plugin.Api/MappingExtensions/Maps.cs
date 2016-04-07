using System;
using System.Collections.Generic;
using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Customers;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

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
                .ForMember(x => x.Product, y => y.MapFrom(src => src.Product.GetWithDefault(x => x, new Product()).ToDto()));
        }

        public static void CreateProductMap()
        {
            Mapper.CreateMap<Product, ProductDto>()
               .IgnoreAllNonExisting()
               .ForMember(x => x.FullDescription, y => y.MapFrom(src => HttpUtility.HtmlEncode(src.FullDescription)));
        }

        public static void CreateOrderItemMap()
        {
            Mapper.CreateMap<OrderItem, OrderItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Product, y => y.MapFrom(src => src.Product.GetWithDefault(x => x, new Product()).ToDto()));
        }

        public static void CreateOrderMap()
        {
            Mapper.CreateMap<OrderDto, Order>().IgnoreAllNonExisting()
                .ForMember(x => x.OrderStatus, y => y.MapFrom(src => Enum.Parse(typeof(OrderStatus), src.OrderStatus.GetWithDefault(x => x, string.Empty), true).ToString()))
                .ForMember(x => x.PaymentStatus, y => y.MapFrom(src => Enum.Parse(typeof(PaymentStatus), src.PaymentStatus.GetWithDefault(x => x, string.Empty), true).ToString()))
                .ForMember(x => x.ShippingStatus, y => y.MapFrom(src => Enum.Parse(typeof(ShippingStatus), src.ShippingStatus.GetWithDefault(x => x, string.Empty), true).ToString()));
        }

        public static void CreateOrderToDTOMap()
        {
            Mapper.CreateMap<Order, OrderDto>().IgnoreAllNonExisting()
                .ForMember(x => x.OrderItems,
                    y =>
                        y.MapFrom(
                            src =>
                                src.OrderItems.GetWithDefault(x => x, new List<OrderItem>())
                                    .Select(item => item.ToDto())))
                .ForMember(x => x.OrderStatus, y => y.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(x => x.PaymentStatus, y => y.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(x => x.ShippingStatus, y => y.MapFrom(src => src.ShippingStatus.ToString()))
                .ForMember(x => x.Customer,
                    y => y.MapFrom(src => src.Customer.GetWithDefault(x => x, new Customer()).ToOrderCustomerDto()))
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));
        }
    }
}