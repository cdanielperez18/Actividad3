import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { TasksComponent } from './components/tasks/tasks.component';
import { authGuard, loginGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/tasks',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [loginGuard]
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [loginGuard]
  },
  {
    path: 'tasks',
    component: TasksComponent,
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: '/tasks'
  }
];
