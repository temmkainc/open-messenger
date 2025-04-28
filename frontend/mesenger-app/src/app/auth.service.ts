import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, switchMap, tap } from 'rxjs';
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
  private userIdKey = 'userId';

  constructor(private http: HttpClient) {}

  login(userCredentials: UserCredentials): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, userCredentials).pipe(
      tap((res) => {
        if (res.accessToken && res.refreshToken && res.id) {
          this.storeTokens(res.accessToken, res.refreshToken, res.id);
        }
      })
    );
  }

  storeTokens(accessToken: string, refreshToken: string, userId: string): void {
    localStorage.setItem(this.accessTokenKey, accessToken);
    localStorage.setItem(this.refreshTokenKey, refreshToken);
    localStorage.setItem(this.userIdKey, userId);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  getUserId(): string | null {
    return localStorage.getItem(this.userIdKey);
  }

  refreshToken(): Observable<{ accessToken: string, refreshToken: string } | null> {
    const refreshToken = this.getRefreshToken();
    const userId = this.getUserId();

    if (!refreshToken || !userId) {
      this.logoutUserLocally();
      return of(null);
    }

    return this.http.post<{ accessToken: string, refreshToken: string }>(
      `${this.apiUrl}/refresh`, { refreshToken, userId }
    ).pipe(
      tap((tokens) => {
        if (tokens) {
          this.storeTokens(tokens.accessToken, tokens.refreshToken, userId);
        }
      })
    );
  }

  logout(logoutDto: LogoutDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/logout`, logoutDto);
  }

  logoutUserLocally(): void {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.userIdKey);
  }

  isLoggedIn(): boolean {
    return this.getAccessToken() !== null;
  }
}
