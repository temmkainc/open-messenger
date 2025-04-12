import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { UserCredentials } from '../models/userCredentials';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/User`;

  constructor(private http: HttpClient) {}

  register(userCredentials: UserCredentials): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userCredentials);
  }

}
