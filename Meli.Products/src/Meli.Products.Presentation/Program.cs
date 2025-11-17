using Meli.Products.Application.Interfaces;
using Meli.Products.Application.Mappings;
using Meli.Products.Application.UseCases;
using Meli.Products.Infrastructure.Persistence;
using Meli.Products.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var webHostEnvironment = builder.Environment;
// Resolve the path to the infrastructure JSON file using a path relative to the Presentation project
var infraPath = Path.Combine(webHostEnvironment.ContentRootPath, "..", "Meli.Products.Infrastructure", "Persistence", "products.json");
var filePath = Path.GetFullPath(infraPath);
builder.Services.AddScoped<IProductRepository>(provider => new ProductRepository(filePath));

builder.Services.AddScoped<GetProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<AddProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();

builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
// OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Meli.Products API", Version = "v1" });
});

// Configurar JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
// Registrar TokenService
builder.Services.AddSingleton<ITokenService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new TokenService(
        config["Jwt:Key"]!,
        config["Jwt:Issuer"]!,
        config["Jwt:Audience"]!
    );
});
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meli.Products API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


