import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { UserCredentials } from '../../models/userCredentials';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  loginFailed: boolean = false;
  errorMessage: string = '';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  onLogin() {
    const user: UserCredentials = {
      email: this.email,
      password: this.password
    };

    this.authService.login(user).subscribe(
      (response) => {
        console.log('User logged in successfully', response);
        this.loginFailed = false;

        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);
        localStorage.setItem('userId', response.id);

        this.router.navigate(['/dashboard']);
      },
      (error) => {
        console.error('Logging in failed', error);
        this.loginFailed = true;
        this.errorMessage = 'Incorrect email or password.';
      }
    );
  }

}
