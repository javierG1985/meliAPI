using Meli.Products.Application.Interfaces;
using Meli.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Products.Application.UseCases
{
    public class UpdateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task ExecuteAsync(Product product)
        {
            return _productRepository.UpdateAsync(product);
        }
    }
}
