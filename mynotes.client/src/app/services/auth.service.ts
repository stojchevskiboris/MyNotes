import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DataService } from './data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(
    private dataService: DataService,
    private toastr: ToastrService
  ) {}

  login(email: string, password: string): Observable<any> {
    return this.dataService.post<any>(`/auth/login`, { email, password })
      .pipe(
        tap({
          next: (response) => {
            this.toastr.success('Login successful', 'Success');
            return response;
          },
          error: (error) => {
            this.toastr.error('Invalid credentials', 'Login Failed');
            throw error;
          }
        })
      );
  }

  // 🌐 Google login
  googleLogin(idToken: string): Observable<any> {
    return this.dataService.post<any>(`/auth/google`, { idToken })
      .pipe(
        tap({
          next: (response) => {
            this.toastr.success('Google login successful', 'Success');
            return response;
          },
          error: (error) => {
            this.toastr.error('Google login failed', 'Error');
            throw error;
          }
        })
      );
  }

  // 🧭 Helper: check if user is logged in
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  // 🚪 Logout user
  logout(): void {
    localStorage.removeItem('token');
    this.toastr.info('Logged out successfully');
  }
}
