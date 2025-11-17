## Meli.Products — README

- Proyecto basado en una arquitectura Onion (Onion Architecture) que implementa principios de arquitectura limpia, promoviendo bajo acoplamiento, separación por responsabilidades y mantenibilidad a largo plazo.
-Esta API de backend simplificada suministrará detalles de productos para ser usados en una función de comparación de ítems. Los datos de los productos se persistirán en un archivo JSON para simular una base de datos.

## Arquitectura General

 - El sistema se organiza en cuatro capas principales:
Presentation → Application → Infrastructure
Application  → Domain
Infrastructure → Application
- Tests → Todas las capas (según tipo de pruebas)

## Estructura del Proyecto
```text
Meli.Products/
├── src/
│   ├── Presentation/
│   │   └── Meli.Products.Presentation/
│   │       ├── Controllers/
│   │       └── Program.cs
│   │
│   ├── Meli.Products.Application/
│   │   ├── UseCases/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Exceptions/
│   │   ├── Validators/
│   │   └── Mappings/
│   │
│   ├── Domain/
│   │   └── Entities/
│   │
│   └── Infrastructure/
│       ├── Persistence/
│       │   └── Repositories/
│       └── Security/
└── tests/
    └── Meli.Products.Presentation.Tests/
        ├── ProductsControllerTests.cs
        └── ProductsControllerSuccessTests.cs
```

## Paquetes NuGet Utilizados
-El proyecto utiliza los siguientes paquetes NuGet:

--FluentValidation — Validación de modelos y reglas de negocio.
--AutoMapper — Mapeo entre entidades y DTOs.
--JWT (System.IdentityModel.Tokens.Jwt) — Implementación de autenticación basada en tokens.
--Swagger / Swashbuckle.AspNetCore — Documentación interactiva de la API.

## Resumen de Funcionalidades
-Presentación (Meli.Products.Presentation)
--AuthController: Implanta autenticación mediante JWT para proteger los endpoints.
--ProductsController: Implementa GET, GET/{id}, POST, PUT y DELETE.
--Gestiona productos almacenados en un archivo JSON.
--Utiliza FluentValidation para validar las solicitudes antes del procesamiento.
--Uso de AutoMapper para transformar entidades a DTOs.
--Manejo robusto de errores mediante:
  ILogger<ProductsController>
  try/catch con respuestas adecuadas
  Middleware global para excepciones no controladas.

| Método | Ruta                 | Descripción                     |
| ------ | -------------------- | ------------------------------- |
| GET    | `/api/Products`      | Lista todos los productos       |
| GET    | `/api/Products/{id}` | Obtiene un producto por ID      |
| POST   | `/api/Products`      | Crea un nuevo producto          |
| PUT    | `/api/Products/{id}` | Actualiza un producto existente |
| DELETE | `/api/Products/{id}` | Elimina un producto existente   |


## Códigos de respuesta relevantes

-200 OK
-201 Created (incluyendo header Location)
-204 No Content
-404 Not Found
-500 Internal Server Error

##Cómo Ejecutar el Proyecto
1. Requisitos previos
-.NET SDK 9.0 o superior
-Editor recomendado: Visual Studio / VS Code
2. Restaurar dependencias
-dotnet restore
3. Construir la solución
-dotnet build
4. Ejecutar la API
-dotnet run --project src/Meli.Products.Presentation
5. Abrir Swagger UI

Una vez ejecutado, ingresa a:
http://localhost:5000/swagger  (el puerto puede variar)


##Pruebas Unitarias
El proyecto incluye pruebas para flujos exitosos, fallos y escenarios not-found.

-Tecnologías usadas:
--xUnit
--Moq
--AutoMapper

tests/
└── Meli.Products.Presentation.Tests/
    ├── ProductsControllerTests.cs          (errores y not-found)
    └── ProductsControllerSuccessTests.cs   (éxitos)

-Ejecutar pruebas
dotnet test

## Notas Técnicas

-Se respetan las firmas originales de los endpoints.
-AutoMapper maneja la conversión entre entidades y DTOs.
-FluentValidation valida las solicitudes antes de llegar al caso de uso.
-El middleware global garantiza respuestas consistentes para errores inesperados.
-La autenticación JWT protege los endpoints de productos.

##Notas Finales

La API está diseñada siguiendo principios RESTful, arquitectura limpia y buenas prácticas para garantizar extensibilidad, testabilidad y mantenibilidad.
