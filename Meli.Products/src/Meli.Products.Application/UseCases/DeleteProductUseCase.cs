using Meli.Products.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Products.Application.UseCases
{
    public class DeleteProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public Task ExecuteAsync(int id)
        {
            return _productRepository.DeleteAsync(id);
        }
    }
}
