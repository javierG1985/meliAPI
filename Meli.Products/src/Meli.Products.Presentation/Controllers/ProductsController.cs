using AutoMapper;
using Microsoft.Extensions.Logging;
using Meli.Products.Application.DTOs;
using Meli.Products.Application.UseCases;
using Meli.Products.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meli.Products.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly GetProductsUseCase _getProductsUseCase;
        private readonly GetProductByIdUseCase _getProductByIdUseCase;
        private readonly AddProductUseCase _addProductUseCase;
        private readonly UpdateProductUseCase _updateProductUseCase;
        private readonly DeleteProductUseCase _deleteProductUseCase;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IMapper mapper,
            GetProductsUseCase getProductsUseCase,
            GetProductByIdUseCase getProductByIdUseCase,
            AddProductUseCase addProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            DeleteProductUseCase deleteProductUseCase,
            ILogger<ProductsController> logger
        )
        {
            _mapper = mapper;
            _getProductsUseCase = getProductsUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
            _addProductUseCase = addProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _deleteProductUseCase = deleteProductUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            try
            {
                // Obtiene products y los mapea a DTOs
                var products = await _getProductsUseCase.ExecuteAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            try
            {
                var product = await _getProductByIdUseCase.ExecuteAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Post(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _addProductUseCase.ExecuteAsync(product);
                return CreatedAtAction(nameof(Get), new { id = product.Id }, _mapper.Map<ProductDto>(product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ProductDto productDto)
        {
            try
            {
                if (id != productDto.Id)
                {
                    return BadRequest();
                }

                var product = _mapper.Map<Product>(productDto);
                await _updateProductUseCase.ExecuteAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _deleteProductUseCase.ExecuteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with id {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }
    }
}


