using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using Meli.Products.Presentation.Controllers;
using Meli.Products.Application.DTOs;
using Meli.Products.Application.Interfaces;
using Meli.Products.Application.UseCases;
using Meli.Products.Domain.Entities;

namespace Meli.Products.Presentation.Tests
{
    public class ProductsControllerSuccessTests
    {
        private static ProductsController CreateControllerForSuccessTests(Mock<IProductRepository> repoMock = null)
        {
            repoMock ??= new Mock<IProductRepository>();

            var getProductsUseCase = new GetProductsUseCase(repoMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repoMock.Object);
            var addProductUseCase = new AddProductUseCase(repoMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repoMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repoMock.Object);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            // Default mappings; individual tests will override as needed
            mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns<IEnumerable<Product>>(src => new List<ProductDto> { new ProductDto { Id = 1, Name = "P1" }, new ProductDto { Id = 2, Name = "P2" } });
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto { Id = 1, Name = "P1" });

            var loggerMock = new Mock<ILogger<ProductsController>>();

            return new ProductsController(
                mapperMock.Object,
                getProductsUseCase,
                getProductByIdUseCase,
                addProductUseCase,
                updateProductUseCase,
                deleteProductUseCase,
                loggerMock.Object
            );
        }

        [Fact]
        public async Task Get_ReturnsOkWithMappedDtos()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>
            {
                new Product { Id = 1, Name = "A" },
                new Product { Id = 2, Name = "B" }
            });

            var controller = CreateControllerForSuccessTests(repoMock);
            // Override mapper behavior for this test
            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(new List<ProductDto> { new ProductDto { Id = 1, Name = "A" }, new ProductDto { Id = 2, Name = "B" } });

            // Re-create controller with the specific mapper behavior
            controller = new ProductsController(
                mapperMock.Object,
                new GetProductsUseCase(repoMock.Object),
                new GetProductByIdUseCase(repoMock.Object),
                new AddProductUseCase(repoMock.Object),
                new UpdateProductUseCase(repoMock.Object),
                new DeleteProductUseCase(repoMock.Object),
                new Mock<ILogger<ProductsController>>().Object
            );

            var result = await controller.Get();
            Assert.Null(result.Result);
            var dtos = Assert.IsType<List<ProductDto>>(result.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal(1, dtos[0].Id);
            Assert.Equal("A", dtos[0].Name);
        }

        [Fact]
        public async Task GetById_ReturnsOkWhenFound()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Product { Id = 42, Name = "X" });

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto { Id = 42, Name = "X" });

            var controller = new ProductsController(
                mapperMock.Object,
                new GetProductsUseCase(repoMock.Object),
                new GetProductByIdUseCase(repoMock.Object),
                new AddProductUseCase(repoMock.Object),
                new UpdateProductUseCase(repoMock.Object),
                new DeleteProductUseCase(repoMock.Object),
                new Mock<ILogger<ProductsController>>().Object
            );

            var result = await controller.Get(42);
            Assert.Null(result.Result);
            var dto = Assert.IsType<ProductDto>(result.Value);
            Assert.Equal(42, dto.Id);
            Assert.Equal("X", dto.Name);
        }

        [Fact]
        public async Task Post_ReturnsCreatedWithDto()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>())).Returns(new Product { Id = 10, Name = "New" });
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto { Id = 10, Name = "New" });

            var controller = new ProductsController(
                mapperMock.Object,
                new GetProductsUseCase(repoMock.Object),
                new GetProductByIdUseCase(repoMock.Object),
                new AddProductUseCase(repoMock.Object),
                new UpdateProductUseCase(repoMock.Object),
                new DeleteProductUseCase(repoMock.Object),
                new Mock<ILogger<ProductsController>>().Object
            );

            var dto = new ProductDto { Id = 0, Name = "New" };
            var result = await controller.Post(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<ProductDto>(result.Value);
            Assert.Equal(10, returned.Id);
            Assert.Equal("New", returned.Name);
            Assert.Equal(nameof(ProductsController.Get), created.ActionName);
        }

        [Fact]
        public async Task Put_ReturnsNoContentOnSuccess()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>())).Returns(new Product { Id = 1, Name = "A" });

            var controller = new ProductsController(
                mapperMock.Object,
                new GetProductsUseCase(repoMock.Object),
                new GetProductByIdUseCase(repoMock.Object),
                new AddProductUseCase(repoMock.Object),
                new UpdateProductUseCase(repoMock.Object),
                new DeleteProductUseCase(repoMock.Object),
                new Mock<ILogger<ProductsController>>().Object
            );

            var dto = new ProductDto { Id = 1, Name = "A" };
            var result = await controller.Put(1, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentOnSuccess()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            var controller = new ProductsController(
                mapperMock.Object,
                new GetProductsUseCase(repoMock.Object),
                new GetProductByIdUseCase(repoMock.Object),
                new AddProductUseCase(repoMock.Object),
                new UpdateProductUseCase(repoMock.Object),
                new DeleteProductUseCase(repoMock.Object),
                new Mock<ILogger<ProductsController>>().Object
            );

            var result = await controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}


