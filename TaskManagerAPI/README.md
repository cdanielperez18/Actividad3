# Task Manager API - Backend

API REST desarrollada en ASP.NET Core para gestión de tareas con autenticación JWT. Implementa persistencia en MySQL y arquitectura por capas con autenticación y autorización basada en tokens.

## Tecnologías Utilizadas

- ASP.NET Core 8.0
- Entity Framework Core
- Pomelo.EntityFrameworkCore.MySql
- MySQL 8.0
- JWT (JSON Web Tokens)
- BCrypt para hash de contraseñas
- Swagger/OpenAPI

## Arquitectura del Proyecto

El proyecto implementa una arquitectura por capas (Layered Architecture) con autenticación centralizada:

```
TaskManagerAPI/
│
├── Controllers/              # Endpoints REST
│   ├── AuthController.cs    # Autenticación y registro
│   └── TasksController.cs   # CRUD de tareas
│
├── Services/                 # Lógica de negocio
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── ITaskService.cs
│   └── TaskService.cs
│
├── Repositories/             # Acceso a datos
│   ├── ITaskRepository.cs
│   ├── TaskRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
│
├── Models/                   # Entidades
│   ├── User.cs
│   └── TaskItem.cs
│ 
├── DTOs/                     # DTOS
│   ├── AuthResponseDto.cs    
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── TaskItemDto.cs
│
├── Data/                     # DbContext
│   └── AppDbContext.cs
│
├── appsettings.json
└── Program.cs
```

### Flujo de Autenticación

```
Cliente → [POST /api/auth/login] → AuthController → AuthService
                                                         ↓
                                            Validar credenciales en MySQL
                                                         ↓
                                            Generar JWT Token → Cliente
                                                         ↓
Cliente → [GET /api/tasks] → [Program.cs] → TasksController
         (Header: Authorization Bearer token)
```

### Flujo de Datos

```
Cliente HTTP → Controller → Service → DbContext → MySQL
                              ↓
                    Porgram.cs JWT valida token
```

## Base de Datos

### Script de Creación

```
CREATE DATABASE TaskManagerDB;
USE TaskManagerDB;

CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Tasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    IsCompleted BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    CompletedAt DATETIME NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_user_id (UserId),
    INDEX idx_is_completed (IsCompleted)
);
```

### Estrategia de Seguridad

- Las contraseñas se hashean con BCrypt antes de almacenarse
- Los tokens JWT tienen expiración configurable (60 minutos por defecto)
- Cada usuario solo puede acceder a sus propias tareas
- Las rutas de tareas están protegidas con [Authorize]
- CORS configurado solo para orígenes permitidos

## Instalación y Configuración

### Requisitos

- .NET 8.0 SDK o superior
- MySQL Server 8.0 o superior

### Configuración

1. Crear la base de datos ejecutando el script SQL proporcionado

2. Configurar `appsettings.json`:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagerDB;User=root;Password=TU_PASSWORD;"
  },
  "JwtSettings": {
    "Secret": "tu_clave_secreta_segura_minimo_32_caracteres_alfanumericos",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerApp",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
3. Restaurar dependencias:
```
dotnet restore
```
4. Aplicar migraciones (si usas EF Migrations):
```
dotnet ef database update
```
5. Ejecutar el proyecto:
```
dotnet run
```
La API estará disponible en `http://localhost:5071` y la documentación Swagger en `http://localhost:5071/swagger`

## Endpoints

### Autenticación

#### POST /api/auth/register

Registrar un nuevo usuario en el sistema.

Parámetros (JSON Body):
- username (string, requerido): Nombre de usuario único
- email (string, requerido): Email válido
- password (string, requerido): Contraseña (mínimo 6 caracteres)

Ejemplo:
```
POST /api/auth/register
Content-Type: application/json

{
  "username": "carlos",
  "email": "carlos@example.com",
  "password": "Carlos123"
}

Respuesta (201 Created):

{
  "id": 1,
  "username": "carlos",
  "email": "carlos@example.com",
  "createdAt": "2026-01-30T21:00:00"
}
```
#### POST /api/auth/login

Iniciar sesión y obtener token JWT.

Parámetros (JSON Body):
- username (string, requerido): Nombre de usuario
- password (string, requerido): Contraseña

Ejemplo:
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "carlos",
  "password": "Carlos123"
}

Respuesta (200 OK):

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNhcmxvcyIsIm5hbWVpZCI6IjEiLCJuYmYiOjE3Mzg...",
  "user": {
    "id": 1,
    "username": "carlos",
    "email": "carlos@example.com"
  }
}
```
### Gestión de Tareas (Requieren Autenticación)

Todas las rutas requieren el header:
Authorization: Bearer {token}

#### GET /api/tasks

Obtener todas las tareas del usuario autenticado.

Ejemplo:
```
GET /api/tasks
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Respuesta (200 OK):

[
  {
    "id": 1,
    "title": "Completar proyecto Angular",
    "description": "Finalizar el Task Manager con autenticación",
    "isCompleted": false,
    "createdAt": "2026-01-30T20:00:00",
    "completedAt": null,
    "userId": 1
  },
  {
    "id": 2,
    "title": "Revisar README",
    "description": "Documentar la API correctamente",
    "isCompleted": true,
    "createdAt": "2026-01-30T19:00:00",
    "completedAt": "2026-01-30T21:00:00",
    "userId": 1
  }
]
```
#### GET /api/tasks/{id}

Obtener detalle de una tarea específica.

Ejemplo:
```
GET /api/tasks/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Respuesta (200 OK):

{
  "id": 1,
  "title": "Completar proyecto Angular",
  "description": "Finalizar el Task Manager con autenticación",
  "isCompleted": false,
  "createdAt": "2026-01-30T20:00:00",
  "completedAt": null,
  "userId": 1
}
```
#### POST /api/tasks

Crear una nueva tarea.

Parámetros (JSON Body):
- title (string, requerido): Título de la tarea
- description (string, opcional): Descripción detallada

Ejemplo:
```
POST /api/tasks
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "Nueva tarea",
  "description": "Descripción de la tarea"
}

Respuesta (201 Created):

{
  "id": 3,
  "title": "Nueva tarea",
  "description": "Descripción de la tarea",
  "isCompleted": false,
  "createdAt": "2026-01-30T21:30:00",
  "completedAt": null,
  "userId": 1
}
```
#### PUT /api/tasks/{id}

Actualizar una tarea existente.

Parámetros (JSON Body - todos opcionales):
- title (string): Nuevo título
- description (string): Nueva descripción
- isCompleted (boolean): Estado de completado

Ejemplo:
```
PUT /api/tasks/3
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "Tarea actualizada",
  "isCompleted": true
}

Respuesta (200 OK):

{
  "id": 3,
  "title": "Tarea actualizada",
  "description": "Descripción de la tarea",
  "isCompleted": true,
  "createdAt": "2026-01-30T21:30:00",
  "completedAt": "2026-01-30T21:35:00",
  "userId": 1
}
```
#### DELETE /api/tasks/{id}

Eliminar una tarea.

Ejemplo:
```
DELETE /api/tasks/3
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Respuesta (204 No Content)
```
## Códigos de Estado HTTP

- 200 OK: Operación exitosa
- 201 Created: Recurso creado exitosamente
- 204 No Content: Eliminación exitosa
- 400 Bad Request: Datos inválidos o faltantes
- 401 Unauthorized: Token inválido o ausente
- 403 Forbidden: Usuario no autorizado para acceder al recurso
- 404 Not Found: Recurso no encontrado
- 500 Internal Server Error: Error del servidor

## Decisiones de Diseño

**Arquitectura por Capas:** Separación clara entre controladores, servicios y acceso a datos. Facilita el mantenimiento, testing y escalabilidad del proyecto.

**JWT para Autenticación:** Implementación stateless que permite escalabilidad horizontal. El token contiene claims del usuario (id, username) para evitar consultas adicionales a la base de datos.

**BCrypt para Contraseñas:** Utiliza un algoritmo de hash robusto con salt automático, protegiendo las contraseñas incluso si la base de datos se ve comprometida.

**Middleware de Errores:** Centraliza el manejo de excepciones, proporcionando respuestas consistentes y evitando exponer detalles internos en producción.

**CORS Configurado:** Permite únicamente peticiones desde http://localhost:4200 (frontend Angular) en desarrollo. En producción debe configurarse el dominio real.

**Cascade Delete:** Al eliminar un usuario, automáticament

## Autor

Carlos Daniel Pérez Serrano  
Prueba Técnica - Desarrollador Web Full Stack