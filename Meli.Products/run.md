# Instrucciones de ejución para la solución Meli.Products 

Este documento explica cómo compilar y ejecutar el proyecto Meli.Products, incluyendo las pruebas.


## Prerequisitos
- .NET 9.0 SDK 
- Familiaridad basica con shell (PowerShell en Windows, bash en Linux/macOS)

## Estructura relevante
- `src/Meli.Products.Presentation` – API controllers
- `src/Meli.Products.Application` – casos de uso y DTOs
- `test/Meli.Products.test` – tests para la aplicación

## Build
1. Restaurar dependencias
   - dotnet restore
2. Construir la aplicación
   - dotnet build

## Ejecutar pruebas unitarias
- Ejecutar todas las pruebas en el proyecto de tests:
  - dotnet test test/Meli.Products.test/Meli.Products.test.csproj
- Ejecutar una prueba específica (por ejemplo, Get_ById_ReturnsOkResult):
  - dotnet test test/Meli.Products.test/Meli.Products.test.csproj --filter FullyQualifiedName=Meli.Products.test.Controllers.ProductsControllerTests.Get_ById_ReturnsOkResult


## Notas y consideraciones
- La prueba para 401 (Unauthorized) no se cubre en pruebas unitarias, ya que la autorización es manejada por el middleware de ASP.NET. Para cubrir 401, usar pruebas de integración con TestServer o WebApplicationFactory.
- Si necesitas ejecutar pruebas de manera repetible, considera limpiar paquetes y caches:
  - dotnet nuget locals all --clear

## Consejos de depuración
- Si una prueba falla, observa el resultado y utiliza filtros para aislar la prueba.
- Asegúrate de que las pruebas están apuntando al proyecto correcto de tests.


