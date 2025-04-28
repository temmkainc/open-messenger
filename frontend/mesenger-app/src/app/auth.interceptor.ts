import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, switchMap, throwError, of } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  let accessToken = authService.getAccessToken();

  if (accessToken) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`
      }
    });
  }

  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        return authService.refreshToken().pipe(
          switchMap((tokens) => {
            if (tokens) {
              console.log("Received new tokens:", tokens); 
              accessToken = tokens.accessToken;
              req = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${accessToken}`
                }
              });
              return next(req);
            } else {
              console.log("No tokens received, logging out...");
              authService.logoutUserLocally();
              return throwError(() => error);
            }
          })
        );
      }
      return throwError(() => error);
    })
  );
};
