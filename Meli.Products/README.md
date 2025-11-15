# Meli.Products - README

Este documento registra y explica los cambios realizados en el proyecto `Meli.Products` durante la entrevista.

## Resumen de cambios realizados
- Manejo básico de errores y comentarios en `ProductsController`:
  - Inyección de `ILogger<ProductsController>` para registrar errores.
  - Envolvimiento de operaciones en try/catch para devolver 500 Internal Server Error en fallos.
  - Respuestas coherentes y mensajes genéricos de error para evitar exponer detalles.
  - Comentarios inline explicando la lógica en puntos clave.
- Pruebas unitarias añadidas:
  - Pruebas de manejo de errores para `ProductsController` (simulación de fallos del repositorio).
  - Pruebas de flujos exitosos para `ProductsController` (Get, Get(id), Post, Put, Delete).
- Colaboración de código:
  - Se introdujo proyecto de pruebas `Meli.Products.Presentation.Tests` con Moq/xUnit.
- Actualización de TODO:
  - Estados marcados como completados para las tareas de errores y pruebas.

## Ubicaciones relevantes
- Archivo principal con cambios de controladores:
  - `Meli.Products/src/Meli.Products.Presentation/Controllers/ProductsController.cs`
    - Enfoque de Get() que mapea a DTOs:
      - Fragmento relevante: `return _mapper.Map<IEnumerable<ProductDto>>(products);`
      - Este comportamiento se mantiene, con el objetivo de garantizar que el flujo de datos al cliente sea consistente con la capa de DTOs.
- Pruebas:
  - Pruebas de errores:
    - `tests/Meli.Products.Presentation.Tests/ProductsControllerTests.cs`
  - Pruebas de éxito:
    - `tests/Meli.Products.Presentation.Tests/ProductsControllerSuccessTests.cs`

## Detalles de código clave
Ejemplo de Get() (flujo exitoso) para referencia:
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
{
    try
    {
        var products = await _getProductsUseCase.ExecuteAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving products");
        return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
    }
}
```

## Cómo ejecutar las pruebas
- Abrir una terminal en la raíz del repositorio.
- Ejecutar:
  - `dotnet test`

## Notas técnicas
- Se mantiene la firma de los endpoints y se añaden pruebas para escenarios de error y de éxito.
- Los tests usan Moq para simular `IProductRepository` y AutoMapper para DTOs.

## Estado de TODO (actualizado)
- Add basic error handling and inline comments to `ProductsController` — COMPLETED
- Write unit tests for error handling in `ProductsController` — COMPLETED
- Add unit tests for successful controller flows in `ProductsController` — COMPLETED
- Document changes in README.md for `ProductsController` and tests — COMPLETED

