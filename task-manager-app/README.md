# Task Manager App - Frontend

Aplicación web SPA (Single Page Application) de gestión de tareas desarrollada en Angular 21 con autenticación JWT. Consume una API REST backend implementando guards, interceptors y arquitectura modular por componentes.

## Tecnologías Utilizadas

- Angular 21.1.2
- TypeScript 5.9.3
- RxJS 7.8.2
- Bootstrap 5.3
- Bootstrap Icons
- Vite (Angular Build)
- FormsModule (Template-driven forms)

## Arquitectura del Proyecto

El proyecto implementa una arquitectura modular con componentes standalone y separación de responsabilidades:

```
task-manager-app/
│
├── src/
│ ├── app/
│ │ ├── components/ # Componentes visuales
│ │ │ ├── login/
│ │ │ │ ├── login.component.ts
│ │ │ │ ├── login.component.html
│ │ │ │ └── login.component.css
│ │ │ ├── register/
│ │ │ │ ├── register.component.ts
│ │ │ │ ├── register.component.html
│ │ │ │ └── register.component.css
│ │ │ ├── tasks/
│ │ │ │ ├── tasks.component.ts
│ │ │ │ ├── tasks.component.html
│ │ │ │ └── tasks.component.css
│ │ │ └── navbar/
│ │ │ ├── navbar.component.ts
│ │ │ ├── navbar.component.html
│ │ │ └── navbar.component.css
│ │ │
│ │ ├── services/ # Lógica de negocio
│ │ │ ├── auth.service.ts
│ │ │ └── task.service.ts
│ │ │
│ │ ├── guards/ # Protección de rutas
│ │ │ └── auth.guard.ts
│ │ │
│ │ ├── interceptors/ # Interceptores HTTP
│ │ │ └── auth.interceptor.ts
│ │ │
│ │ ├── models/ # Interfaces y tipos
│ │ │ ├── user.model.ts
│ │ │ └── task.model.ts
│ │ │
│ │ ├── app.component.ts # Componente raíz
│ │ ├── app.config.ts # Configuración de la app
│ │ └── app.routes.ts # Definición de rutas
│ │
│ ├── index.html
│ ├── main.ts
│ └── styles.css
│
├── angular.json
├── package.json
└── tsconfig.json
```

### Flujo de Autenticación

```
    Usuario → [/login] → LoginComponent
    ↓

    Submit → AuthService.login() → POST /api/auth/login
    ↓

    API devuelve {token, user} → localStorage.setItem('token')
    ↓

    Router.navigate(['/tasks']) → AuthGuard verifica token
    ↓

    TasksComponent → TaskService.getAllTasks()
    ↓

    AuthInterceptor → Agrega header: Authorization Bearer {token}
    ↓

    API valida token → Devuelve tareas del usuario
```

### Flujo de Peticiones HTTP

```
Usuario acción → Component → Service → HTTP Client
↓
AuthInterceptor
↓
Agrega Authorization header
↓
API Backend (ASP.NET Core)
↓
Respuesta / Error 401
↓
AuthInterceptor maneja error
↓
Redirige a /login si no autorizado
```

## Instalación y Configuración

### Requisitos

- Node.js 18.x o superior
- npm 9.x o superior
- Angular CLI 21.x

### Configuración

1. Instalar dependencias:

```bash
npm install
```

2. Configurar URL del backend en `pokemon.service.ts`:

```typescript
private apiUrl = 'http://localhost:5071/api/auth;
```

3. Ejecutar en modo desarrollo:
```
ng serve
```

La aplicación estará disponible en `http://localhost:4200`

5. Build de producción:
```
ng build --configuration production
```

Los archivos optimizados se generarán en `dist/task-manager-app/browser/`

## Rutas de la Aplicación
```
| Ruta       | Estado    | Componente        | Descripción                          |
|------------|-----------|-------------------|--------------------------------------|
| /          | Pública   | Redirect          | Redirecciona a /login                |
| /login     | Pública   | LoginComponent    | Inicio de sesión                     |
| /register  | Pública   | RegisterComponent | Registro de usuario                  |
| /tasks     | Protegida | TasksComponent    | Gestión de tareas (requiere auth)    |
```

### Configuración de Rutas (app.routes.ts)
```
export const routes: Routes = [
{ path: '', redirectTo: '/login', pathMatch: 'full' },
{ path: 'login', component: LoginComponent },
{ path: 'register', component: RegisterComponent },
{
path: 'tasks',
component: TasksComponent,
canActivate: [AuthGuard] // Protegida
},
{ path: '**', redirectTo: '/login' }
];
```

## Componentes Principales

### LoginComponent

Formulario de inicio de sesión con validación.

Funcionalidades:
- Validación de campos requeridos
- Manejo de errores de autenticación
- Loading state durante login
- Redirección automática a /tasks después de login exitoso
- Link a registro

Campos:
- Username (requerido)
- Password (requerido)

### RegisterComponent

Formulario de registro de nuevos usuarios.

Funcionalidades:
- Validación de email
- Validación de contraseña (mínimo 6 caracteres)
- Confirmación de contraseña
- Verificación de username único
- Redirección a /login después de registro exitoso

Campos:
- Username (requerido, único)
- Email (requerido, formato email)
- Password (requerido, mínimo 6 caracteres)
- Confirm Password (debe coincidir)

### TasksComponent

Gestión completa de tareas con interfaz tipo dashboard.

Funcionalidades:
- Listar todas las tareas del usuario
- Crear nueva tarea (modal Bootstrap)
- Editar tarea existente (modal Bootstrap)
- Marcar tarea como completada/pendiente (checkbox)
- Eliminar tarea (modal de confirmación)
- Estadísticas en tiempo real (pendientes/completadas)
- Indicadores visuales (badges de estado)
- Formato de fechas en español

Modales:
- Modal Crear/Editar: Formulario con título y descripción
- Modal Eliminar: Confirmación antes de eliminar

### NavbarComponent

Barra de navegación responsive con Bootstrap.

Funcionalidades:
- Logo y título de la aplicación
- Mostrar nombre del usuario autenticado
- Botón de cerrar sesión
- Responsive con menú hamburguesa en móvil
- Se muestra solo en rutas autenticadas

## Servicios

### AuthService

```
Gestiona toda la lógica de autenticación.

Métodos públicos:

register(data: RegisterRequest): Observable<User>
// Registra un nuevo usuario

login(credentials: LoginRequest): Observable<AuthResponse>
// Inicia sesión y guarda el token

logout(): void
// Cierra sesión, elimina token y redirige a login

getToken(): string | null
// Obtiene el token actual de localStorage

isAuthenticated(): boolean
// Verifica si el usuario está autenticado

getCurrentUser(): User | null
// Obtiene datos del usuario desde localStorage
```

Almacenamiento:
- Token JWT en localStorage con key 'token'
- Datos de usuario en localStorage con key 'user'

### TaskService

```
Gestiona las operaciones CRUD de tareas.

Métodos públicos:

getAllTasks(): Observable<Task[]>
// Obtiene todas las tareas del usuario autenticado

getTaskById(id: number): Observable<Task>
// Obtiene una tarea específica por ID

createTask(data: CreateTaskRequest): Observable<Task>
// Crea una nueva tarea

updateTask(id: number, data: UpdateTaskRequest): Observable<Task>
// Actualiza título, descripción o estado de una tarea

deleteTask(id: number): Observable<void>
// Elimina una tarea permanentemente
```

Nota: Todas las peticiones son interceptadas automáticamente por AuthInterceptor para agregar el token.

## Guards e Interceptors

### AuthGuard (auth.guard.ts)

Guard que protege rutas que requieren autenticación.

Funcionamiento:
1. Se ejecuta antes de activar una ruta protegida
2. Verifica si existe token en localStorage
3. Si hay token: permite acceso (return true)
4. Si no hay token: redirige a /login (return false)

Uso:

```
{
path: 'tasks',
component: TasksComponent,
canActivate: [AuthGuard]
}
```

### AuthInterceptor (auth.interceptor.ts)

Interceptor HTTP que agrega automáticamente el token de autorización.

Funcionamiento:
1. Intercepta TODAS las peticiones HTTP salientes
2. Obtiene el token de AuthService
3. Si hay token: clona el request y agrega header Authorization
4. Si no hay token: permite el request original (rutas públicas)
5. Maneja errores 401: elimina token y redirige a login

Implementación:

```
export const authInterceptor: HttpInterceptorFn = (req, next) => {
const authService = inject(AuthService);
const router = inject(Router);
const token = authService.getToken();

if (token) {
req = req.clone({
setHeaders: {
Authorization: Bearer ${token}
}
});
}

return next(req).pipe(
catchError((error: HttpErrorResponse) => {
if (error.status === 401) {
authService.logout();
}
return throwError(() => error);
})
);
};
```

## Modelos de Datos

### User (user.model.ts)

```
export interface User {
id: number;
username: string;
email: string;
createdAt: string;
}

export interface LoginRequest {
username: string;
password: string;
}

export interface RegisterRequest {
username: string;
email: string;
password: string;
}

export interface AuthResponse {
token: string;
user: User;
}
```

### Task (task.model.ts)
```
export interface Task {
id: number;
title: string;
description: string;
isCompleted: boolean;
createdAt: string;
completedAt: string | null;
userId: number;
}

export interface CreateTaskRequest {
title: string;
description: string;
}

export interface UpdateTaskRequest {
title?: string;
description?: string;
isCompleted?: boolean;
}
```

## Estilos y UI

### Framework CSS
```
Bootstrap 5.3.0 integrado en `angular.json`:

"styles": [
"src/styles.css",
"node_modules/bootstrap/dist/css/bootstrap.min.css"
],
"scripts": [
"node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]
```

### Componentes Bootstrap Utilizados

- Cards: Para mostrar tareas individuales
- Modals: Para crear, editar y eliminar
- Forms: Formularios de login, registro y tareas
- Badges: Indicadores de estado (Pendiente/Completada)
- Buttons: Botones con variantes (primary, danger, secondary)
- Navbar: Barra de navegación responsive
- Spinners: Loading states

### Responsive Design

- Grid system de Bootstrap (col-md-6, col-lg-4)
- Breakpoints: Mobile first
- Navbar colapsable en dispositivos pequeños
- Modales centrados y responsivos

## Manejo de Errores

### Errores de Autenticación

- 401 Unauthorized: Token inválido o expirado → Logout automático
- 400 Bad Request: Credenciales incorrectas → Mensaje al usuario
- 409 Conflict: Username ya existe → Mensaje al usuario

### Errores en Tareas

- 404 Not Found: Tarea no existe → Alert al usuario
- 403 Forbidden: Usuario no autorizado → Alert al usuario
- 500 Server Error: Error del servidor → Mensaje genérico

### Loading States

Todos los componentes implementan:

```
isLoading = false;

loadData() {
this.isLoading = true;
this.service.getData().subscribe({
next: (data) => {
// Procesar datos
this.isLoading = false;
},
error: (error) => {
// Manejar error
this.isLoading = false;
}
});
}
```

## Decisiones de Diseño

**Componentes Standalone:** Angular 21 utiliza componentes standalone por defecto, eliminando la necesidad de NgModules. Esto simplifica la estructura y mejora el tree-shaking.

**Guards Funcionales:** Implementación de guards como funciones en lugar de clases, siguiendo las mejores prácticas de Angular moderno.

**Interceptors Funcionales:** El nuevo formato funcional de interceptors es más conciso y fácil de testear que la implementación basada en clases.

**LocalStorage para Token:** Persistencia del token en localStorage permite mantener la sesión incluso después de recargar la página. Para mayor seguridad en producción, considerar httpOnly cookies.

**Template-driven Forms:** Se utilizan formularios basados en templates con ngModel para una implementación rápida. Para formularios más complejos, considerar Reactive Forms.

**Modales Bootstrap Nativos:** Implementación manual de modales Bootstrap sin dependencias adicionales como ng-bootstrap, reduciendo el tamaño del bundle.

**Separación de Concerns:** Cada componente tiene su propia lógica, servicios para comunicación con API, guards para seguridad e interceptors para operaciones transversales.

**RxJS para Asincronía:** Uso de Observables para todas las operaciones asíncronas, permitiendo mejor manejo de errores y cancelación de peticiones.

## Autor

Carlos Daniel Pérez Serrano  
Prueba Técnica - Desarrollador Web Full Stack