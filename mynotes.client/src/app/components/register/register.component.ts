import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  loading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.loading = true;
    this.errorMessage = '';
    this.authService.register(this.username, this.email, this.password).subscribe({
      next: (res) => {
        localStorage.setItem('token', res.token);
        this.router.navigate(['/notes']); // adjust if you have a different default route
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error || 'Registration failed.';
      }
    });
  }
}
