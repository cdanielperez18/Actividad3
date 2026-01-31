# Task Manager Full Stack

Aplicación Full Stack de gestión de tareas con autenticación JWT que implementa un sistema completo de registro, login y CRUD de tareas personales. Backend desarrollado en ASP.NET Core con persistencia en MySQL y frontend en Angular con guards e interceptors.

## Descripción

Este proyecto demuestra la implementación de una arquitectura completa tipo BaaS (Backend as a Service) que incluye:

- Sistema de autenticación y autorización con JWT
- Backend RESTful con arquitectura por capas
- Persistencia de datos en base de datos relacional
- Frontend SPA moderno con framework Angular
- Protección de rutas con guards
- Interceptores HTTP para autorización automática
- Manejo centralizado de errores y estados

## Arquitectura General

```
┌─────────────────────┐
│ Angular 21 │
│ (Frontend SPA) │
│ │
│ - Login/Register │
│ - AuthGuard │
│ - AuthInterceptor │
│ - Task CRUD │
└──────────┬──────────┘
│
│ HTTP Request/Response
│ Authorization: Bearer {JWT}
↓
┌─────────────────────┐
│ ASP.NET Core 8.0 │
│ (Backend API) │
│ │
│ - JWT Auth │
│ - Controllers │
│ - Services │
│ - Repositories │
└──────────┬──────────┘
│
│ Entity Framework Core
↓
┌─────────────────────┐
│ MySQL 8.0 │
│ (Database) │
│ │
│ - Users │
│ - Tasks │
└─────────────────────┘
```

## Stack Tecnológico

**Backend:**
- ASP.NET Core 8.0
- Entity Framework Core
- Pomelo.EntityFrameworkCore.MySql
- MySQL 8.0
- JWT (JSON Web Tokens)
- BCrypt para hash de contraseñas
- Swagger/OpenAPI

**Frontend:**
- Angular 21.1.2
- TypeScript 5.9.3
- RxJS 7.8.2
- Bootstrap 5.3
- FormsModule
- Vite (Angular Build)

## Estructura del Proyecto
```
task-manager-fullstack/
│
├── TaskManagerAPI/
│ ├── Controllers/
│ │ ├── AuthController.cs
│ │ └── TasksController.cs
│ ├── Services/
│ │ ├── IAuthService.cs
│ │ ├── AuthService.cs
│ │ ├── ITaskService.cs
│ │ └── TaskService.cs
│ ├── Repositories/
│ │ ├── ITaskRepository.cs
│ │ ├── TaskRepository.cs
│ │ ├── IUserRepository.cs
│ │ └── UserRepository.cs
│ ├── Models/
│ │ ├── User.cs
│ │ └── TaskItem.cs
│ ├── DTOs/
│ │ ├── AuthResponseDto.cs
│ │ ├── LoginDto.cs
│ │ ├── RegisterDto.cs
│ │ └── TaskItemDto.cs
│ ├── Data/
│ │ └── AppDbContext.cs
│ ├── appsettings.json
│ └── README.md
│
└── task-manager-app/
├── src/app/
│ ├── components/
│ │ ├── login/
│ │ ├── register/
│ │ ├── tasks/
│ │ └── navbar/
│ ├── services/
│ │ ├── auth.service.ts
│ │ └── task.service.ts
│ ├── guards/
│ │ └── auth.guard.ts
│ ├── interceptors/
│ │ └── auth.interceptor.ts
│ ├── models/
│ │ ├── user.model.ts
│ │ └── task.model.ts
│ └── app.routes.ts
└── README.md
```

## Instalación y Ejecución

### Paso 1: Configurar Base de Datos

1. Abrir MySQL Workbench o cliente MySQL
2. Ejecutar el script SQL:

```sql
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

    Verificar que la base de datos TaskManagerDB se haya creado correctamente

### Paso 2: Configurar y Ejecutar Backend

    Navegar a la carpeta del backend:
```
cd TaskManagerAPI
```

    Configurar appsettings.json:
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
  }
}
```

    Restaurar dependencias y ejecutar:
```
dotnet restore
dotnet run

```
Verificar que el servidor esté disponible en http://localhost:5071

### Paso 3: Configurar y Ejecutar Frontend

En una terminal separada:

    Navegar a la carpeta del frontend:
```
cd task-manager-app
```

    Instalar dependencias:
```
npm install
```

    Ejecutar en modo desarrollo:
```
ng serve
```

Acceder a la aplicación en http://localhost:4200

### Funcionalidades Implementadas

### Requisitos Funcionales

- Login con usuario y contraseña
- Registro de nuevos usuarios
- Emisión y almacenamiento de token JWT
- CRUD completo de tareas (Crear, Leer, Actualizar, Eliminar)
- Marcar tareas como completadas/pendientes
- Estadísticas de tareas (pendientes/completadas)
- Protección de rutas privadas
- Cierre de sesión con limpieza de token
- Mostrar usuario autenticado en navbar

### Requisitos Técnicos

- Backend desarrollado en ASP.NET Core 8.0
- Base de datos MySQL con tablas Users y Tasks
- Endpoints autenticados mediante token JWT
- Frontend consumiendo exclusivamente el backend
- HTTP Interceptor para agregar header Authorization Bearer automáticamente
- AuthGuard para protección de rutas
- Manejo centralizado de errores de autenticación (401, 403)
- Modales personalizados de Bootstrap
- Responsive design con Bootstrap 5
- Componentes standalone de Angular 21

### Flujo de Autenticación

### 1. Registro de Usuario
```
Usuario → Formulario de Registro → POST /api/auth/register
                                            ↓
                                    Backend hashea password (BCrypt)
                                            ↓
                                    Guarda usuario en MySQL
                                            ↓
                                    Retorna usuario creado → Redirige a /login
```

### 2. Inicio de Sesión
```
Usuario → Formulario de Login → POST /api/auth/login
                                        ↓
                                Backend valida credenciales
                                        ↓
                                Genera token JWT
                                        ↓
                                Retorna {token, user}
                                        ↓
                                Frontend guarda en localStorage
                                        ↓
                                Redirige a /tasks
```

### 3. Peticiones Autenticadas
```
Usuario acción en /tasks → Component → Service → HTTP Request
                                                        ↓
                                                AuthInterceptor
                                                        ↓
                                        Agrega: Authorization: Bearer {token}
                                                        ↓
                                                Backend valida JWT
                                                        ↓
                                                Retorna datos del usuario
```

### 4. Token Expirado o Inválido
```
HTTP Request → Backend retorna 401 Unauthorized
                        ↓
                AuthInterceptor captura error
                        ↓
                Elimina token de localStorage
                        ↓
                Redirige a /login
```

### Endpoints Principales

Autenticación
```
    POST /api/auth/register - Registrar nuevo usuario

    POST /api/auth/login - Iniciar sesión y obtener JWT
```
Tareas (Requieren Autenticación)
```
    GET /api/tasks - Listar tareas del usuario autenticado

    GET /api/tasks/{id} - Obtener tarea específica

    POST /api/tasks - Crear nueva tarea

    PUT /api/tasks/{id} - Actualizar tarea

    DELETE /api/tasks/{id} - Eliminar tarea
```
Todas las rutas de tareas requieren el header:
```
Authorization: Bearer {token}
```

### Decisiones de Arquitectura

Arquitectura por Capas (Backend): Separación clara entre Controllers, Services, Repositories y Data Access. Permite mejor mantenimiento, testing y escalabilidad.

JWT Stateless: No se almacena estado de sesión en el servidor, permitiendo escalabilidad horizontal y reduciendo carga del servidor.

Guards e Interceptors (Frontend): Centraliza la lógica de autenticación y autorización, evitando código duplicado en componentes.

LocalStorage para Token: Permite persistencia de sesión entre recargas de página. En producción se recomienda evaluar httpOnly cookies por seguridad.

BCrypt para Passwords: Hash robusto con salt automático que protege contraseñas incluso si la base de datos se ve comprometida.

Bootstrap 5: Framework CSS maduro que acelera el desarrollo UI con componentes responsivos sin sacrificar personalización.

Componentes Standalone Angular 21: Eliminan la necesidad de NgModules, simplificando la estructura y mejorando tree-shaking para bundles más pequeños.

## Documentación Adicional

- [Backend README](./TaskManagerAPI/README.md) - Detalles de arquitectura, endpoints, autenticación y configuración
- [Frontend README](./task-manager-app/README.md) - Componentes, servicios, guards, interceptors y estructura

Seguridad Implementada

- Contraseñas hasheadas con BCrypt (salt automático)
- Tokens JWT firmados con clave secreta
- Validación de datos en todos los endpoints
- CORS configurado para origen específico
- Protección contra inyección SQL (EF Core parametrizado)
- Expiración de tokens configurable (60 minutos por defecto)
- Autorización a nivel de usuario (cada usuario solo ve sus tareas)
- Manejo de errores sin exponer detalles internos

## Autor

Carlos Daniel Pérez Serrano
Desarrollador Web Full Stack
Enero 2026