import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, switchMap } from 'rxjs';
import { environment } from '../environments/environment';
import { UserCredentials } from '../models/userCredentials';
import { jwtDecode } from 'jwt-decode';
import { LogoutDto } from '../models/logoutDto';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/User`;
  private accessTokenKey = 'accessToken';
  private refreshTokenKey = 'refreshToken';

  constructor(private http: HttpClient) {}

  getUserFromToken(): Observable<any> {
    const token = localStorage.getItem(this.accessTokenKey);
    console.log('Access Token retrieved from localStorage:', token); 
  
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        const currentTime = Math.floor(Date.now() / 1000);
  
        if (decodedToken.exp < currentTime) {
          console.log('Token has expired, refreshing...');
          
          const refreshToken = localStorage.getItem(this.refreshTokenKey);
          const userId = decodedToken.sub;
  
          if (!refreshToken || !userId) {
            console.log('No refreshToken or userId found in localStorage');
            this.logoutUserLocally();
            return of(null);
          }
  
          console.log('Sending refresh request...');
          return this.http.post<{ accessToken: string; refreshToken: string }>(
            `${this.apiUrl}/refresh`,
            { userId, refreshToken }
          ).pipe(
            switchMap((tokens) => {
              if (tokens) {
                console.log('Received new tokens:', tokens);
                this.storeTokens(tokens.accessToken, tokens.refreshToken);
  
                const newDecoded: any = jwtDecode(tokens.accessToken);
                return of({
                  id: newDecoded.sub,
                  email: newDecoded.email,
                  username: newDecoded.username,
                });
              } else {
                console.log('Failed to refresh tokens');
                this.logoutUserLocally();
                return of(null);
              }
            })
          );
        }
  
        console.log('Access token is valid, decoding...');
        return of({
          id: decodedToken.sub,
          email: decodedToken.email,
          username: decodedToken.username,
        });
      } catch (error) {
        console.error('Failed to decode token', error);
        this.logoutUserLocally();
        return of(null);
      }
    }
  
    console.log('No token found in localStorage. Returning null.');
    return of(null);
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

  refreshAccessToken(refreshToken: string, userId: string) {
    if (refreshToken && userId) {
      return this.http.post<{ accessToken: string, refreshToken: string }>(
        `${this.apiUrl}/refresh`,
        { refreshToken, userId }
      );
    }
    return of(null);
  }
  

  
  logoutUserLocally(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }
}
