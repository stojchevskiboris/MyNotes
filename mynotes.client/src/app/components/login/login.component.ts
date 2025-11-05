import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  loading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngAfterViewInit() {
    (window as any).handleGoogleResponse = (response: any) => {
      const idToken = response.credential;
      this.authService.googleLogin(idToken).subscribe({
        next: (res) => {
          localStorage.setItem('token', res.token);
          this.router.navigate(['/notes']);
        },
        error: (err) => {
          console.error('Google login failed', err);
          this.errorMessage = 'Google login failed. Please try again.';
        }
      });
    };
  }

  login() {
    this.loading = true;
    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        localStorage.setItem('token', res.token);
        this.router.navigate(['/notes']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = 'Invalid email or password.';
      }
    });
  }
}
