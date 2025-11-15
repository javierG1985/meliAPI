using Meli.Products.Application.Interfaces;
using Meli.Products.Domain.Entities;
 using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Meli.Products.Infrastructure.Persistence
{
    public class ProductRepository: IProductRepository
    {
        private readonly string _filePath;

        public ProductRepository(string filePath)
        {
            _filePath = filePath;
        }

        private async Task<List<Product>> ReadProductsAsync()
        {
            // Return an empty list if the file doesn't exist.
            if (!File.Exists(_filePath))
            {
                return new List<Product>();
            }

            // Read the file content and deserialize it into a list of products.
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
        }



        // Writes a list of products to the JSON file.
        private async Task WriteProductsAsync(List<Product> products)
        {
            // Serialize the list of products and write it to the file.
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Returns all products from the JSON file.
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await ReadProductsAsync();
        }

        // Returns a single product by its ID.
        public async Task<Product?> GetByIdAsync(int id)
        {
            var products = await ReadProductsAsync();
            return products.FirstOrDefault(p => p.Id == id);
        }

        // Adds a new product to the JSON file.
        public async Task AddAsync(Product product)
        {
            var products = await ReadProductsAsync();

            // Assign a new ID to the product.
            product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);

            await WriteProductsAsync(products);
        }

        // Updates an existing product in the JSON file.
        public async Task UpdateAsync(Product product)
        {
            var products = await ReadProductsAsync();
            var existingProduct = products.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {            // Update the existing product's properties.
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Rating = product.Rating;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Specifications = product.Specifications;

                await WriteProductsAsync(products);
            }
        }

        // Deletes a product from the JSON file.
        public async Task DeleteAsync(int id)
        {
            var products = await ReadProductsAsync();
            var productToRemove = products.FirstOrDefault(p => p.Id == id);

            if (productToRemove != null)
            {
                products.Remove(productToRemove);
                await WriteProductsAsync(products);
            }
        }
    }
}
