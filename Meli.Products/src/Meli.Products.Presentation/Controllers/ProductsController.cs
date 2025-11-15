using AutoMapper;
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

        public ProductsController(
            IMapper mapper,
            GetProductsUseCase getProductsUseCase,
            GetProductByIdUseCase getProductByIdUseCase,
            AddProductUseCase addProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            DeleteProductUseCase deleteProductUseCase
        )
        {
            _mapper = mapper;
            _getProductsUseCase = getProductsUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
            _addProductUseCase = addProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _deleteProductUseCase = deleteProductUseCase;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> Get()
        {
            var products = await _getProductsUseCase.ExecuteAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var product = await _getProductByIdUseCase.ExecuteAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return _mapper.Map<ProductDto>(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Post(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _addProductUseCase.ExecuteAsync(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, _mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);
            await _updateProductUseCase.ExecuteAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _deleteProductUseCase.ExecuteAsync(id);
            return NoContent();
        }
    }
}


