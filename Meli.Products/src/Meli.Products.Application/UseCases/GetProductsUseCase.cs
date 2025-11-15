using Meli.Products.Application.Interfaces;
using Meli.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Products.Application.UseCases
{
    public class GetProductsUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetProductsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IEnumerable<Product>> ExecuteAsync()
        {
            return _productRepository.GetAllAsync();
        }
    }
}
