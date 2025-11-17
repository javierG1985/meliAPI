using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Meli.Products.Presentation.Controllers;
using Meli.Products.Application.DTOs;
using Meli.Products.Domain.Entities;
using Meli.Products.Application.Interfaces;
using Meli.Products.Application.UseCases;
using FluentValidation.Results;

namespace Meli.Products.test.Controllers
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            // Arrange
            var sampleProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Silla Gamer",
                    Price = 500,
                    Description = "Desc A",
                    Rating = 4,
                    ImageUrl = "http://example.com/a.png",
                    Specifications = new Dictionary<string, string> { { "Color", "Rojo" } }
                }
            };

            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(sampleProducts);

            // Use case instances with the mocked repository
            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> src) => src.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description ?? string.Empty,
                    Rating = p.Rating,
                    ImageUrl = p.ImageUrl ?? string.Empty,
                    Specifications = p.Specifications ?? new Dictionary<string, string>()
                }));

            var loggerMock = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                loggerMock.Object
            );

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            var dtoList = dtos.ToList();
            Assert.Single(dtoList);
            Assert.Equal(1, dtoList[0].Id);
            Assert.Equal("Silla Gamer", dtoList[0].Name);
        }

        [Fact]
        public async Task Get_ById_ReturnsOkResult()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            var existingProduct = new Product
            {
                Id = 2,
                Name = "portatil",
                Price = 200,
                Description = "prueba unitaria",
                Rating = 4,
                ImageUrl = "http://example.com/b.png",
                Specifications = new Dictionary<string, string> { { "Color", "Azul" }, { "memoria", "4gb" } }
            };
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(existingProduct);

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description ?? string.Empty,
                    Rating = p.Rating,
                    ImageUrl = p.ImageUrl ?? string.Empty,
                    Specifications = p.Specifications ?? new Dictionary<string, string>()
                });
            // (Mapping for CreateProductDto not required in this test)
            var loggerMock = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                loggerMock.Object
            );

            // Act
            var result = await controller.Get(2);

            // Assert
            var dto = (result.Result as OkObjectResult)?.Value as ProductDto ?? result.Value as ProductDto;
            Assert.NotNull(dto);
            Assert.Equal(2, dto!.Id);
            Assert.Equal("portatil", dto.Name);
        }

        [Fact]
        public async Task Get_ById_Returns500OnException()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new System.Exception("boom"));

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns((Product p) => new ProductDto { Id = p.Id });
            var loggerMock = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                loggerMock.Object
            );

            // Act
            var result = await controller.Get(999);

            // Assert
            var status = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task Post_Returns201Created_OnSuccess()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => p.Id = 123)
                .Returns(Task.CompletedTask);

            var createDto = new CreateProductDto
            {
                Name = "Nuevo producto",
                Price = 10m,
                Description = "desc",
                Rating = 4.0,
                ImageUrl = "http://image",
                Specifications = new Dictionary<string, string>()
            };

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>()))
                .Returns((CreateProductDto dto) => new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    Rating = dto.Rating,
                    ImageUrl = dto.ImageUrl,
                    Specifications = dto.Specifications
                });
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Rating = p.Rating,
                    ImageUrl = p.ImageUrl,
                    Specifications = p.Specifications
                });

            var loggerMock = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                loggerMock.Object
            );

            // Act
            var actionResult = await controller.Post(createDto);
            var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var dto = Assert.IsType<ProductDto>(created.Value);

            // Assert
            Assert.Equal(123, dto.Id);
        }

        [Fact]
        public async Task Post_Returns400()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            var invalidDto = new CreateProductDto { Name = string.Empty, Price = 0m };

            // Act
            var result = await controller.Post(invalidDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task Put_Returns204OnSuccess()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Old", Price = 5, Description = "d" });
            repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>()))
                .Returns((ProductDto dto) => new Product
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    Rating = dto.Rating,
                    ImageUrl = dto.ImageUrl,
                    Specifications = dto.Specifications
                });

            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            var updateDto = new ProductDto { Id = 1, Name = "Updated", Price = 10, Description = "d", Rating = 4.5, ImageUrl = "http", Specifications = new Dictionary<string, string>() };

            // Act
            var result = await controller.Put(1, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Put_Returns400_WhenIdMismatch()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            var dto = new ProductDto { Id = 2 };

            // Act
            var result = await controller.Put(1, dto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Put_Returns500()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });
            repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ThrowsAsync(new System.Exception("boom"));

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>()))
                .Returns((ProductDto dto) => new Product { Id = dto.Id, Name = dto.Name, Price = dto.Price, Description = dto.Description, Rating = dto.Rating, ImageUrl = dto.ImageUrl, Specifications = dto.Specifications });

            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            var dto = new ProductDto { Id = 1, Name = "X" };

            // Act
            var result = await controller.Put(1, dto);

            // Assert
            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task Delete_Returns204_OnSuccess()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });
            repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns500()
        {
            // Arrange
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });
            repositoryMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new System.Exception("boom"));

            var getProductsUseCase = new GetProductsUseCase(repositoryMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repositoryMock.Object);
            var addProductUseCase = new AddProductUseCase(repositoryMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repositoryMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repositoryMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            var controller = new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                new Mock<ILogger<ProductsController>>().Object
            );

            // Act
            var result = await controller.Delete(1);

            // Assert
            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}



