using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    public class ProductsControllerTests
    {
        private static ProductsController CreateControllerForErrorTests(Mock<IProductRepository> repoMock = null)
        {
            // Preparar mocks comunes
            repoMock ??= new Mock<IProductRepository>();

            // UseCases reales con repositorio simulado (errores serán provocados por el repositorio)
            var getProductsUseCase = new GetProductsUseCase(repoMock.Object);
            var getProductByIdUseCase = new GetProductByIdUseCase(repoMock.Object);
            var addProductUseCase = new AddProductUseCase(repoMock.Object);
            var updateProductUseCase = new UpdateProductUseCase(repoMock.Object);
            var deleteProductUseCase = new DeleteProductUseCase(repoMock.Object);

            // Mapper y logger simulados
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(new List<ProductDto>());
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

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
        public async Task Get_WhenRepositoryThrows_Returns500()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("boom"));

            var controller = CreateControllerForErrorTests(repoMock);

            var result = await controller.Get();

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Error retrieving data", objectResult.Value);
        }

        [Fact]
        public async Task GetById_WhenRepositoryThrows_Returns500()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("boom"));

            var controller = CreateControllerForErrorTests(repoMock);

            var result = await controller.Get(1);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Error retrieving data", objectResult.Value);
        }

        [Fact]
        public async Task Post_WhenRepositoryThrows_Returns500()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("boom"));

            var controller = CreateControllerForErrorTests(repoMock);
            var dto = new ProductDto { Id = 0, Name = "Test" };

            var result = await controller.Post(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Error creating data", objectResult.Value);
        }

        [Fact]
        public async Task Put_WhenRepositoryThrows_Returns500()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("boom"));

            var controller = CreateControllerForErrorTests(repoMock);
            var dto = new ProductDto { Id = 1, Name = "Test" };

            var result = await controller.Put(1, dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Error updating data", objectResult.Value);
        }

        [Fact]
        public async Task Delete_WhenRepositoryThrows_Returns500()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new Exception("boom"));

            var controller = CreateControllerForErrorTests(repoMock);

            var result = await controller.Delete(1);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Error deleting data", objectResult.Value);
        }
    }
}

