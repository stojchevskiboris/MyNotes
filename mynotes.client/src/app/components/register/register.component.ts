import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpStatusCode } from '@angular/common/http';
import { AppMessages } from '../../models/shared/app-messages';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  
  model = {
    name: '',
    email: '',
    password: ''
  };

  loading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.loading = true;
    this.errorMessage = '';
    this.authService.register(this.model).subscribe({
      next: (res) => {
        localStorage.setItem('token', res.token);
        this.router.navigate(['/notes']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.status == HttpStatusCode.BadRequest ? err.error : AppMessages.UnexpectedError;
      }
    });
  }
}
