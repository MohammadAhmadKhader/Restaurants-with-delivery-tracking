using FluentValidation;
using Orders.Contracts.Dtos.Order;
using Shared.Extensions;

namespace Orders.Extensions.FluentValidationValidators;

class OrderPlaceDtoValidator : AbstractValidator<OrderPlaceDto>
{
    public OrderPlaceDtoValidator()
    {
        RuleFor(o => o.DeliveryAddressId)
        .NotNull()
        .NotEmpty();

        RuleFor(x => x.Items)
        .NotEmpty()
        .NotNull();

        RuleFor(x => x.Items)
        .MustHaveUniqueValues(x => x.ItemId)
        .WithMessage("Duplicate items are not allowed.");

        RuleForEach(x => x.Items)
         .SetValidator(new ItemValidator());
    }
    
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(x => x.ItemId)
                .GreaterThan(0);

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}

