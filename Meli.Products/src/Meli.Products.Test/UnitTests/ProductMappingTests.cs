using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Meli.Products.Domain.Entities;
using Meli.Products.Application.DTOs;
using Meli.Products.Application.Mappings;
using Xunit;

namespace Meli.Products.Test
{
    public class ProductMappingTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>();
            });
            return config.CreateMapper();
        }

        [Fact]
        public void Product_to_ProductDto_maps_correctly()
        {
            // Arrange
            var mapper = CreateMapper();
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200m,
                Description = "A high-performance laptop for all your needs.",
                Rating = 4.5,
                ImageUrl = "https://via.placeholder.com/150",
                Specifications = new Dictionary<string, string> { { "CPU", "Intel Core i7" }, { "RAM", "16GB" }, { "Storage", "512GB SSD" } }
            };

            // Act
            var dto = mapper.Map<ProductDto>(product);

            // Assert
            dto.Should().BeEquivalentTo(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Rating = product.Rating,
                ImageUrl = product.ImageUrl,
                Specifications = product.Specifications
            });
        }

        [Fact]
        public void ProductDto_to_Product_maps_correctly()
        {
            // Arrange
            var dto = new ProductDto
            {
                Id = 2,
                Name = "Mouse",
                Price = 25m,
                Description = "A comfortable and responsive mouse.",
                Rating = 4.7,
                ImageUrl = "https://via.placeholder.com/150",
                Specifications = new Dictionary<string, string> { { "Type", "Wireless" }, { "DPI", "1600" } }
            };
            var mapper = CreateMapper();

            // Act
            var product = mapper.Map<Product>(dto);

            // Assert
            product.Should().BeEquivalentTo(new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Rating = dto.Rating,
                ImageUrl = dto.ImageUrl,
                Specifications = dto.Specifications
            });
        }
    }
}

