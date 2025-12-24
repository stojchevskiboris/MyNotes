import { Component, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpStatusCode } from '@angular/common/http';
import { AppMessages } from '../../models/shared/app-messages';

declare const google: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements AfterViewInit {
  model = {
    email: '',
    password: ''
  }
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
          this.errorMessage = AppMessages.GoogleLoginFailed;
        }
      });
    };

    setTimeout(() => {
      const googleDiv = document.querySelector('.g_id_signin');
      if (googleDiv) {
        google.accounts.id.initialize({
          client_id: this.authService.googleClientId,
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
    this.authService.login(this.model).subscribe({
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
