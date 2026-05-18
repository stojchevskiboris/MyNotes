import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private loginEndpoint = `/Auth/Login`;
  private registerEndpoint = `/Auth/Register`;
  private googleLoginEndpoint = `/Auth/GoogleLogin`;
  private updateUserEndpoint = `/Auth/UpdateUser`;

  googleClientId = '19935988541-uftril2pfdatkoij0o5vu56t7j5e6ttp.apps.googleusercontent.com';
  currentUser: any = null;
  token: string = '';
  userId: string = '';

  constructor(
    private dataService: DataService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  login(model: any): Observable<any> {
    return this.dataService.post<any>(this.loginEndpoint, model)
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
  register(model: any): Observable<any> {
    return this.dataService.post<any>(this.registerEndpoint, model)
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
            return response;
          },
          error: (error) => {
            throw error;
          }
        })
      );
  }

  updateUser(model: any): Observable<any> {
    return this.dataService.post<any>(this.updateUserEndpoint, model)
      .pipe(
        tap({
          next: (response) => {
            localStorage.setItem('currentUser', JSON.stringify(response));
            this.currentUser = response;
            this.toastr.success('Settings updated successfully');
            return response;
          },
          error: (error) => {
            this.toastr.error('Failed to update settings');
            throw error;
          }
        })
      );
  }

  get googleClientID(): string {
    return this.googleClientId;
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUserId(): number {
    return isNaN(Number(localStorage.getItem('userId'))) ? 0 : Number(localStorage.getItem('userId'));
  }

  getUser(): any {
    const userStr = localStorage.getItem('currentUser');
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch (e) {
        return null;
      }
    }
    return null;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('userId');
    this.currentUser = null;
    this.token = '';
    this.userId = '';
    this.toastr.info('Logged out successfully');
    this.router.navigate(['/login']);
  }
  
  storeUserCredentials(response: any) {
    this.token = response.token;
    this.currentUser = response.user;
    this.userId = response.userId;
    localStorage.setItem('token', this.token);
    localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
    localStorage.setItem('userId', this.userId.toString());
  }
}
