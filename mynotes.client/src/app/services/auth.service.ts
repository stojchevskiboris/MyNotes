import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private loginEndpoint = `/Auth/Login`;
  private registerEndpoint = `/Auth/Register`;
  private googleLoginEndpoint = `/Auth/GoogleLogin`;
  
  currentUser: string = '';
  token: string = '';
  userId: string = '';

  constructor(
    private dataService: DataService,
    private toastr: ToastrService
  ) {}

  login(email: string, password: string): Observable<any> {
    return this.dataService.post<any>(this.loginEndpoint, { email, password })
      .pipe(
        tap({
          next: (response) => {
            this.storeUserCredentials(response);
            return response;
          },
          error: (error) => {
            throw error;
          }
        })
      );
  }

  // Register new local user
  register(username: string, email: string, password: string): Observable<any> {
    return this.dataService.post<any>(this.registerEndpoint, { username, email, password })
      .pipe(
        tap({
          next: (response) => {
            this.storeUserCredentials(response);
            return response;
          },
          error: (error) => {
            throw error;
          }
        })
      );
  }

  // Google login
  googleLogin(idToken: string): Observable<any> {
    return this.dataService.post<any>(this.googleLoginEndpoint, { idToken })
      .pipe(
        tap({
          next: (response) => {            
            this.storeUserCredentials(response);
            this.toastr.info('Google login successful');
            return response;
          },
          error: (error) => {
            this.toastr.error('Google login failed', 'Error');
            throw error;
          }
        })
      );
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUserId(): number {
    return isNaN(Number(localStorage.getItem('userId'))) ? 0 : Number(localStorage.getItem('userId'));
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('userId');
    this.currentUser = '';
    this.token = '';
    this.userId = '';
    this.toastr.info('Logged out successfully');
  }
  
  storeUserCredentials(response: any) {
    this.token = response.token;
    this.currentUser = response.user;
    this.userId = response.userId;
    localStorage.setItem('token', this.token);
    localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
    localStorage.setItem('userId', this.userId);
  }
}