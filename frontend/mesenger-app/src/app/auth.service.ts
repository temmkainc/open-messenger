import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { UserCredentials } from '../models/userCredentials';
import { jwtDecode } from 'jwt-decode';
import { LogoutDto } from '../models/logoutDto';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/User`;
  private accessTokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';

  constructor(private http: HttpClient) {}

  getUserFromToken(): any {
    const token = localStorage.getItem('accessToken');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        const currentTime = Math.floor(Date.now() / 1000);
  
        if (decodedToken.exp < currentTime) {
          localStorage.removeItem('accessToken');
          return null;
        }

        return { id: decodedToken.sub, email: decodedToken.email, username: decodedToken.username };
      } catch (error) {
        console.error('Failed to decode token', error);
        return null;
      }
    }
    return null;
  }
  

  login(userCredentials: UserCredentials): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, userCredentials);
  }

  storeTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem(this.accessTokenKey, accessToken);
    localStorage.setItem(this.refreshTokenKey, refreshToken);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  logout(logoutDto: LogoutDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/logout`, logoutDto);
  }

  isLoggedIn(): boolean {
    return this.getAccessToken() !== null;
  }
}
