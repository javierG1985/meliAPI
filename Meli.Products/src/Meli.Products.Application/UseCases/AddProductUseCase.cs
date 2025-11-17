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
    public class AddProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public AddProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task ExecuteAsync(Product product)
        {
            return _productRepository.AddAsync(product);
        }
    }
}
