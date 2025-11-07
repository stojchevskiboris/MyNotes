import { Component, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

declare const google: any; // required to access Google script

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements AfterViewInit {
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

    setTimeout(() => {
      const googleDiv = document.querySelector('.g_id_signin');
      if (googleDiv) {
        google.accounts.id.initialize({
          client_id: '19935988541-uftril2pfdatkoij0o5vu56t7j5e6ttp.apps.googleusercontent.com',
          callback: (response: any) => (window as any).handleGoogleResponse(response)
        });
        google.accounts.id.renderButton(googleDiv, {
          type: googleDiv.getAttribute('data-type') ?? 'standard',
          theme: googleDiv.getAttribute('data-theme') ?? 'outline',
          size: googleDiv.getAttribute('data-size') ?? 'large',
          text: googleDiv.getAttribute('data-text') ?? 'signin_with',
          shape: googleDiv.getAttribute('data-shape') ?? 'rectangular',
          logo_alignment: googleDiv.getAttribute('data-logo_alignment') ?? 'center'
        });      
      }
    }, 50);
  }

  login() {
    this.loading = true;
    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        localStorage.setItem('token', res.token);
        this.router.navigate(['/notes']);
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Invalid email or password.';
      }
    });
  }
}
