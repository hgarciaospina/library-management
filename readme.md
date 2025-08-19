# 📚 Library Management System

Un sistema completo de gestión de bibliotecas desarrollado con .NET 8, Entity Framework Core y arquitectura limpia.

## 🚀 Características

- Gestión completa de libros, miembros, préstamos y bibliotecas
- API RESTful con documentación Swagger integrada
- Interfaz web con Razor Pages
- Validación robusta con FluentValidation
- Base de datos SQL Server creada automáticamente con EF Core
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

1. Clonar el repositorio:

```bash
git clone <url-del-repositorio>
cd LibraryManagement
```

2. Abrir la solución en Visual Studio (`LibraryManagement.sln`)  
   Espera a que se restauren los paquetes NuGet.

3. Configurar proyecto de inicio:  
   Haz clic derecho en el proyecto `LibraryManagement.Api` → "Set as Startup Project".

4. Ejecutar la aplicación: F5 o Ctrl + F5  

Al ejecutar:

✅ Se crea automáticamente la base de datos `LibraryDB` y todas las tablas  
✅ Se insertan datos de prueba (seeding inicial)  
✅ Swagger UI disponible en [http://localhost:5230/swagger](http://localhost:5230/swagger)  
✅ Servidor web corriendo en [http://localhost:5230](http://localhost:5230)

---

## ⚠️ Solución de problemas de base de datos

Si falla la creación de la base de datos, ejecuta este script en SQL Server Management Studio (SSMS):

```sql
USE master;
GO
-- Habilita el login de tu usuario
ALTER LOGIN [TU_MAQUINA\TU_USUARIO] ENABLE;
GO
-- Otorga permisos de administrador (solo para desarrollo)
EXEC sp_addsrvrolemember 'TU_MAQUINA\TU_USUARIO', 'sysadmin';
GO
```

Para identificar tu usuario:

- Abre una terminal y ejecuta:

```bash
whoami
```

- O en Windows: Presiona `Win + R`, escribe `cmd` y ejecuta:

```cmd
echo %USERDOMAIN%\%USERNAME%
```

Ejemplo real:

```sql
USE master;
GO
ALTER LOGIN [DESKTOP-ABC123\juanperez] ENABLE;
GO
EXEC sp_addsrvrolemember 'DESKTOP-ABC123\juanperez', 'sysadmin';
GO
```

Si las tablas ya se crearon pero Swagger no carga:  
Detén la ejecución, cierra el navegador y vuelve a ejecutar `LibraryManagement.Api`. Swagger debería cargar automáticamente.

---

## 🌐 Acceso a la aplicación

- **API Documentation (Swagger):** [http://localhost:5230/swagger](http://localhost:5230/swagger)  
- **Interfaz Web (Frontend):** [http://localhost:5230](http://localhost:5230)

## 🖥️ Ejecutar el Frontend (LibraryManagement.Web)

Después de que la API esté funcionando:

1. Detén la ejecución de la API.  
2. Haz clic derecho en el proyecto `LibraryManagement.Web` → "Set as Startup Project".  
3. Presiona F5 para ejecutar.

### 🎯 Menú del Frontend:

- MEMBERS → Gestión de miembros  
- LIBRARIES → Gestión de bibliotecas  
- BOOKS → Gestión de libros  
- LOANS → Gestión de préstamos

---

## 🗄️ Estructura de la base de datos creada

| Tabla     | Descripción           | Registros de ejemplo  |
|-----------|---------------------|---------------------|
| Books     | Libros del sistema   | 10+ libros          |
| Members   | Miembros registrados | 5+ miembros         |
| Loans     | Préstamos activos    | 3+ préstamos        |
| Libraries | Bibliotecas/sucursales | 2+ bibliotecas  |

---

## 🔧 Configuración alternativa de connection strings

- SQL Server Express:

```json
"Server=.\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=true;TrustServerCertificate=True;"
```

- SQL Server Local:

```json
"Server=localhost;Database=LibraryDB;Trusted_Connection=true;TrustServerCertificate=True;"
```

- SQL Server con autenticación SQL:

```json
"Server=localhost;Database=LibraryDB;User Id=sa;Password=tu_password;TrustServerCertificate=True;"
```

---

## 📞 Soporte técnico

Verifica:

- SQL Server ejecutándose  
- Permisos de tu usuario de Windows  
- Connection string correcta en `appsettings.json`

Errores comunes:

- **Login failed for user:** Ejecuta el script de permisos en SSMS  
- **Cannot create database:** Verifica permisos de administrador  
- **Swagger not loading:** Ejecuta el proyecto de nuevo

---

## ✅ Checklist de puesta en marcha

- [ ] Visual Studio 2022 instalado  
- [ ] .NET 8 SDK instalado  
- [ ] SQL Server instalado y ejecutándose  
- [ ] Repositorio clonado  
- [ ] Solución abierta en Visual Studio  
- [ ] `LibraryManagement.Api` como proyecto de inicio  
- [ ] Aplicación ejecutada con F5  
- [ ] Base de datos `LibraryDB` creada automáticamente  
- [ ] Datos de prueba insertados  
- [ ] Swagger accesible en `/swagger`  
- [ ] Frontend ejecutado (`LibraryManagement.Web`)

¡Feliz coding! 🎉

