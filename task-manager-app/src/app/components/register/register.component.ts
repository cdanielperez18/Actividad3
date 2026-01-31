import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    // Validaciones
    if (!this.username || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Por favor completa todos los campos';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Las contraseñas no coinciden';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'La contraseña debe tener al menos 6 caracteres';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const registerData: RegisterRequest = {
      username: this.username,
      email: this.email,
      password: this.password
    };

    this.authService.register(registerData).subscribe({
      next: () => {
        this.router.navigate(['/tasks']);
      },
      error: (error) => {
        console.error('Error en registro:', error);
        this.errorMessage = error.error?.message || 'Error al registrar usuario. Verifica que el usuario o email no estén en uso';
        this.isLoading = false;
      }
    });
  }
}
