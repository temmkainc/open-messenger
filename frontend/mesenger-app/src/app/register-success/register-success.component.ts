import { Component } from '@angular/core';
import { RouterModule, Router, ActivatedRoute  } from '@angular/router';

@Component({
  selector: 'app-register-success',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './register-success.component.html',
  styleUrl: './register-success.component.css'
})
export class RegisterSuccessComponent {
  constructor(
    private router : Router,
    private route : ActivatedRoute
  ) {}
}
