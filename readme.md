# 📚 Library Management System

Un sistema completo de gestión de bibliotecas desarrollado con .NET 8, Entity Framework Core y arquitectura limpia.

## 🚀 Características

- Gestión completa de libros, miembros, préstamos y bibliotecas  
- API RESTful con documentación Swagger integrada  
- Interfaz web con Razor Pages  
- Validación robusta con FluentValidation  
- Base de datos SQL Server creada automáticamente al ejecutar el proyecto  
- Arquitectura en capas (Clean Architecture)  

## 📋 Prerrequisitos

Software necesario:  

- Visual Studio 2022 (versión 17.8 o superior)  
- .NET 8 SDK  
- SQL Server (Express, LocalDB o Developer Edition)  
- SQL Server Management Studio (SSMS - opcional pero recomendado)  
- Git para clonar el repositorio  

Verificar instalaciones:

```bash
dotnet --version
# Debe mostrar: 8.0.x o superior

sqlcmd -S localhost -Q "SELECT @@VERSION"
# Debe mostrar información de SQL Server
```

## 🛠️ Configuración inicial

1. Clonar el repositorio  

```bash
git clone <url-del-repositorio>
cd LibraryManagement
```

2. Abrir la solución en Visual Studio  
   - Abre `LibraryManagement.sln`  
   - Espera a que se restauren los paquetes NuGet  

3. Ejecutar la aplicación  

### 🚀 Ejecutar la API

1. Configurar proyecto de inicio:  
   - Haz clic derecho en `LibraryManagement.Api`  
   - Selecciona "Set as Startup Project"  

2. Ejecutar con F5 o Ctrl + F5  

**📊 Lo que sucede al ejecutar:**

- ✅ Se crea la base de datos automáticamente llamada `LibraryDB`  
- ✅ Se generan todas las tablas automáticamente  
- ✅ Se insertan datos de prueba (seeding inicial)  
- ✅ Se inicia el servidor web en `http://localhost:5230`  
- ✅ Swagger UI disponible en `http://localhost:5230/swagger`  

### 🌐 Acceso a la aplicación

- **API Documentation (Swagger)**: `http://localhost:5230/swagger`  
- **Interfaz Web (Frontend)**: `http://localhost:5230`  

### 🖥️ Ejecutar el Frontend (LibraryManagement.Web)

1. Después de que la API esté funcionando:  
   - Detén la ejecución de la API  
   - Haz clic derecho en `LibraryManagement.Web`  
   - Selecciona "Set as Startup Project"  
   - Presiona F5 para ejecutar  

2. Menú del Frontend:  

- **MEMBERS** - Gestión de miembros  
- **LIBRARYS** - Gestión de bibliotecas  
- **BOOKS** - Gestión de libros  
- **LOANS** - Gestión de préstamos  

### 📊 Verificar datos en la base de datos

```sql
-- Ejecuta en SSMS para verificar datos
USE LibraryDB;
SELECT * FROM Books;
SELECT * FROM Members;
SELECT * FROM Libraries;
SELECT * FROM Loans;
```

### 🗄️ Estructura de la base de datos creada

| Tabla     | Descripción              | Registros de ejemplo |
|-----------|-------------------------|-------------------|
| Books     | Libros del sistema      | 10+ libros        |
| Members   | Miembros registrados    | 5+ miembros       |
| Loans     | Préstamos activos       | 3+ préstamos      |
| Libraries | Bibliotecas/sucursales  | 2+ bibliotecas    |

## 📞 Soporte técnico

- Verifica que SQL Server esté ejecutándose  
- Comprueba los permisos de tu usuario de Windows  
- Errores comunes y soluciones:  
  - **"Login failed for user"**: asegúrate de tener permisos suficientes  
  - **"Cannot create database"**: verifica que tu usuario tenga permisos de creación  
  - **"Swagger not loading"**: detén y ejecuta de nuevo `LibraryManagement.Api`  

## ✅ Checklist de puesta en marcha

- Visual Studio 2022 instalado  
- .NET 8 SDK instalado  
- SQL Server instalado y ejecutándose  
- Repositorio clonado  
- Solución abierta en Visual Studio  
- `LibraryManagement.Api` establecido como startup  
- Aplicación ejecutada con F5  
- Base de datos `LibraryDB` creada automáticamente  
- Datos de prueba insertados (verificar en SSMS)  
- Swagger accesible en `/swagger`  
- Frontend ejecutado (`LibraryManagement.Web`)  

¡Feliz coding! 🎉

