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
            FinancialStatus = null;
            FulfillmentStatus = null;
            CustomerId = null;
        }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? FinancialStatus { get; set; }
        public ShippingStatus? FulfillmentStatus { get; set; }
        public int? CustomerId { get; set; }
    }
}