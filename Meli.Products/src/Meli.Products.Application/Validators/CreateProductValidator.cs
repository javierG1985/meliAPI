using FluentValidation;
using Meli.Products.Application.DTOs;

namespace Meli.Products.Application.Validators
{
    public class CreateProductValidator: AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().Length(2, 100);
            RuleFor(p => p.Price).GreaterThan(0);
            RuleFor(p => p.Description).MaximumLength(500);
            RuleFor(p => p.Rating).InclusiveBetween(0, 5);
            RuleFor(p => p.ImageUrl).MaximumLength(2000).When(p => p.ImageUrl != null);
            RuleFor(p => p.Specifications).NotNull().WithMessage("Specifications cannot be null");
        }

    }
}
