﻿using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.Validators
{
    public class ShoppingCartItemDtoValidator : AbstractValidator<ShoppingCartItemDto>
    {
        public ShoppingCartItemDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(x => x.CustomerId)
                    .NotNull()
                    .WithMessage("Please, set customer id");

                RuleFor(x => x.ProductId)
                    .NotNull()
                    .WithMessage("Please, set product id");

                RuleFor(x => x.Quantity)
                    .NotNull()
                    .WithMessage("Please, set quantity");
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                int parsedId = 0;

                RuleFor(x => x.Id)
                        .NotNull()
                        .NotEmpty()
                        .Must(id => int.TryParse(id, out parsedId) && parsedId > 0)
                        .WithMessage("Invalid Id");

                if (passedPropertyValuePaires.ContainsKey("customer_id"))
                {
                    RuleFor(x => x.CustomerId)
                      .NotNull()
                      .WithMessage("Please, set customer id");
                }

                if (passedPropertyValuePaires.ContainsKey("product_id"))
                {
                    RuleFor(x => x.ProductId)
                      .NotNull()
                      .WithMessage("Please, set product id");
                }

                if (passedPropertyValuePaires.ContainsKey("quantity"))
                {
                    RuleFor(x => x.Quantity)
                       .NotNull()
                       .WithMessage("Please, set quantity");
                }
            }

            if (passedPropertyValuePaires.ContainsKey("rental_start_date_utc") || passedPropertyValuePaires.ContainsKey("rental_end_date_utc"))
            {
                RuleFor(x => x.RentalStartDateUtc)
                    .NotNull()
                    .WithMessage("Please provide a rental start date");

                RuleFor(x => x.RentalEndDateUtc)
                   .NotNull()
                   .WithMessage("Please provide a rental end date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalStartDateUtc < dto.RentalEndDateUtc)
                    .WithMessage("Rental start date should be before rental end date");
            }
        }
    }
}