import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpParamsOptions } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: any = null;
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService, private router: Router, 
    private http: HttpClient) {}
    ngOnInit(): void {
      this.authService.getUserFromToken().subscribe({
        next: (userDetails) => {
          if (userDetails) {
            this.user = userDetails;
            this.isLoggedIn = true;
          } else {
            this.isLoggedIn = false;
            this.router.navigate(['/login']);
          }
        },
        error: (err) => {
          console.error('Error retrieving user from token:', err);
          this.isLoggedIn = false;
          this.router.navigate(['/login']);
        }
      });
    }

  onLogout() {
    const logoutDto = { userId: this.user.id };
    this.authService.logout(logoutDto).subscribe(
      response => {
        console.log('Logout successful:', response);
        
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        
        this.router.navigate(['/login']);
      },
      error => {
        console.error('Logout failed:', error);
      }
    );
  }
  
}
