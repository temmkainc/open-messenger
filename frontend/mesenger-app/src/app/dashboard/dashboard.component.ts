import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

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
  private apiUrl = `${environment.apiUrl}/User`; 

  constructor(
    private authService: AuthService,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.http.get<any>(`${this.apiUrl}/profile`)
    .subscribe({
      next: (user) => {
        this.user = user;
        this.isLoggedIn = true;
      },
      error: (err) => {
        console.error('Profile load failed:', err);
        this.isLoggedIn = false;
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      }
    });
  }

  onLogout() {
    const userId = this.authService.getUserId();
    if (!userId) {
      console.error('No userId found in storage. Logging out locally.');
      this.authService.logoutUserLocally();
      this.router.navigate(['/login']);
      return;
    }

    const logoutDto = { userId: this.user.id};

    this.authService.logout(logoutDto).subscribe({
      next: (response) => {
        console.log('Logout successful:', response);
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Logout failed:', error);
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      }
    });
  }
}
