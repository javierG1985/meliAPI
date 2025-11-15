using AutoMapper;
using Meli.Products.Domain.Entities;
using Meli.Products.Application.DTOs;

namespace Meli.Products.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}

