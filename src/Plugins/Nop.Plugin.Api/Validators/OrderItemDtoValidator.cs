using FluentValidation;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.Validators
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                    .NotNull()
                    .WithMessage("Invalid product id");

            RuleFor(x => x.Quantity)
                   .NotNull()
                   .Must(quantity => quantity > 0)
                   .WithMessage("Invalid quantity");
        }
    }
}