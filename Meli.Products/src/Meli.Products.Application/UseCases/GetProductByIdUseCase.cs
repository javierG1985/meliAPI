using Meli.Products.Application.Exceptions;
using Meli.Products.Application.Interfaces;
using Meli.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Products.Application.UseCases
{
    public class GetProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product?> ExecuteAsync(int id)
        {
            var product = _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw ApiException.NotFound($"Producto con ID {id} no encontrado");
            }
            return product;
        }
    }
}
