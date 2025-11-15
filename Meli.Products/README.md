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
- Pruebas adicionales añadidas:
  - GetById not-found (retorna 404) cuando no existe el recurso.
  - GetById éxito (retorna 200 OK con DTO).
  - Delete éxito (retorna 204 No Content).

## Visión general del proyecto (en español)

Este proyecto gestiona productos mediante una API REST basada en una arquitectura en capas: Presentación, Aplicación, Dominio e Infraestructura.

### Arquitectura y responsabilidades
- Presentación: `Meli.Products.Presentation` (Controllers).
- Aplicación: UseCases; `IProductRepository` como abstracción de acceso a datos.
- Dominio: Entidades (p. ej., `Product`).
- Infraestructura: Repositorio de persistencia (p. ej., `ProductRepository`).
- Mapeo: `ProductDto` para DTOs y `ProductMappingProfile` para AutoMapper.
- Pruebas: pruebas unitarias para errores y para flujos exitosos; pruebas de not-found para GetById, PUT y DELETE.

### Endpoints RESTful de la API (resumen)
- GET `api/Products` — lista de productos.
- GET `api/Products/{id}` — producto por ID (NotFound si no existe).
- POST `api/Products` — crea un nuevo producto.
- PUT `api/Products/{id}` — actualiza un producto (verificación de existencia previa).
- DELETE `api/Products/{id}` — elimina un producto (verificación de existencia previa).

### Pruebas unitarias (en español)
- Cobertura:
  - Errores en cada endpoint (Get, GetById, Post, Put, Delete).
  - Flujos exitosos (Get, GetById, Post, Put, Delete).
  - Not-found para GetById, PUT y DELETE.
- Herramientas: XUnit, Moq, AutoMapper.
- Estructura de pruebas:
  - `tests/Meli.Products.Presentation.Tests/ProductsControllerTests.cs` (errores y not-found)
  - `tests/Meli.Products.Presentation.Tests/ProductsControllerSuccessTests.cs` (éxitos)

### Cómo ejecutar las pruebas
- Abre una terminal en la raíz del repositorio.
- Ejecuta: `dotnet test`

## Notas finales
- La API sigue principios RESTful y utiliza un modelo por capas para facilitar el mantenimiento.
- Si deseas, puedo ampliar la documentación con ejemplos de respuestas HTTP detalladas para cada caso.
- Pruebas adicionales añadidas:
  - GetById not-found (retorna 404) cuando no existe el recurso.
  - GetById éxito (retorna 200 OK con DTO).
  - Delete éxito (retorna 204 No Content).

## Ejemplos de respuestas HTTP (RESTful)

- GET api/Products — 200 OK
  - Cuerpo de ejemplo:
  ```json
  [
    {"id":1,"name":"Producto A","price":9.99,"description":"Descripcion","rating":4.5,"imageUrl":"","specifications":{}}
  ]
  ```
- GET api/Products/{id} — 200 OK
  - Cuerpo de ejemplo:
  ```json
  {"id":1,"name":"Producto A","price":9.99,"description":"Descripcion","rating":4.5,"imageUrl":"","specifications":{}}
  ```
- GET api/Products/{id} — 404 Not Found
  - Sin cuerpo (o vacío).
- POST api/Products — 201 Created
  - Cuerpo de ejemplo:
  ```json
  {"id":2,"name":"Producto B","price":19.99,"description":"Descripcion","rating":4.0,"imageUrl":"","specifications":{}}
  ```
  - Encabezado Location: /api/Products/2
- PUT api/Products/{id} — 204 No Content (caso exitoso)
- PUT api/Products/{id} — 404 Not Found (si no existe antes de actualizar)
- DELETE api/Products/{id} — 204 No Content (caso exitoso)
- DELETE api/Products/{id} — 404 Not Found (si no existe)

## Estado de TODO (actualizado)
- Add basic error handling and inline comments to `ProductsController` — COMPLETED
- Write unit tests for error handling in `ProductsController` — COMPLETED
- Add unit tests for successful controller flows in `ProductsController` — COMPLETED
- Document changes in README.md for `ProductsController` and tests — COMPLETED
 
### Ejemplos de peticiones HTTP con curl

- GET all: 
```bash
curl -sS -X GET "https://localhost:5001/api/Products" -H "Accept: application/json"
```
- GET by id (ej. 1):
```bash
curl -sS -X GET "https://localhost:5001/api/Products/1" -H "Accept: application/json"
```
- POST (crear):
```bash
curl -sS -X POST "https://localhost:5001/api/Products" \
  -H "Content-Type: application/json" \
  -d '{"name":"Producto Nuevo","price":9.99,"description":"Descripcion","rating":4.5,"imageUrl":"","specifications":{}}'
```
- PUT (actualizar):
```bash
curl -sS -X PUT "https://localhost:5001/api/Products/1" \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Producto Actualizado","price":19.99,"description":"Descripcion actualizada","rating":4.6,"imageUrl":"","specifications":{}}'
```
- DELETE (eliminar) 1:
```bash
curl -sS -X DELETE "https://localhost:5001/api/Products/1"
```

