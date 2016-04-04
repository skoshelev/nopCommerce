using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Api.Models.OrdersParameters
{
    public class BaseOrdersParametersModel
    {
        public BaseOrdersParametersModel()
        {
            CreatedAtMax = null;
            CreatedAtMin = null;
            Status = null;
            PaymentStatus = null;
            ShippingStatus = null;
            CustomerId = null;
        }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public ShippingStatus? ShippingStatus { get; set; }
        public int? CustomerId { get; set; }
    }
}