using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using Xunit;
using Meli.Products.Application.DTOs;
using Meli.Products.Application.Validators;

namespace Meli.Products.test.Validators
{
    public class CreateProductValidatorTests
    {
        [Fact]
        public void InvalidDto_WithMultipleErrors()
        {
            // Arrange
            var invalidDto = new CreateProductDto
            {
                Name = "",
                Price = -5m,
                Description = new string('a', 501),
                Rating = 6,
                ImageUrl = new string('x', 2100),
                Specifications = null
            };
            var validator = new CreateProductValidator();

            // Act
            ValidationResult result = validator.Validate(invalidDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            // Ensure at least one error is present for common fields
            Assert.Contains(result.Errors, e => e.PropertyName == "Name" || e.PropertyName == "Price" || e.PropertyName == "Description");
        }
    }
}


