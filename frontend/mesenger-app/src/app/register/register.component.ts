import { Component } from '@angular/core';
import { UserService } from '../user.service';
import { User } from '../../models/user';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute  } from '@angular/router';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule,RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent {
  email: string = '';
  password: string = '';
  username: string = '';

  constructor(
    private userService: UserService,
    private router : Router,
    private route : ActivatedRoute
  ) {}

  onRegister(): void {
    const user: User = {
      email: this.email,
      password: this.password,
      username: this.username,
    };
  
    this.userService.register(user).subscribe(
      (response) => {
        console.log('User registered successfully', response);
        this.router.navigate(['/successfulRegistration']);
      },
      (error) => {
        console.error('Registration failed', error);
      }
    );
  }
  
}
