import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service'; // Assuming your AuthService is correctly set up
import { switchMap, catchError, of } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

export const loggerInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  console.log('Request is on its way:', req);

  return next(req);
  
};
