using AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class OrderItemDtoMappings
    {
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return Mapper.DynamicMap<OrderItem, OrderItemDto>(orderItem);
        }
    }
}