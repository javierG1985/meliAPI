# API de Productos - Documentación

Este proyecto expone una API REST para la gestión de productos.

Base de la API
- Ruta base: `/api/Products`
- Este README documenta la API expuesta por la capa de Presentación
  (`Meli.Products.Presentacion`).

Modelos
- `ProductDto`
  - `Id`: int
  - `Name`: string
  - `Price`: decimal
  - `Description`: string
  - `Rating`: double
  - `ImageUrl`: string
  - `Specifications`: Dictionary<string, string>

Persistencia local
- Los datos se almacenan en: `src/Meli.Products.Infrastructure/Persistence/products.json`
- El repositorio `ProductRepository` lee/escribe ese archivo para las operaciones CRUD.

EndPoints implementados
- `GET /api/Products`
  - Retorna una lista de `ProductDto`.

Notas
- Este README asume que la aplicación se ejecuta en un host de desarrollo (Kestrel) y escucha en `http://localhost:5000` por defecto. El puerto puede variar según la configuración.
- Para ejecutar:
  - Navega a la carpeta del proyecto de Presentación.
  - Ejecuta: `dotnet run`
- Pruebas rápidas:
  - `curl -s http://localhost:5000/api/Products`


