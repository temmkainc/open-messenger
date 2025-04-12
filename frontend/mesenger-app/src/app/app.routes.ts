import { Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { RegisterSuccessComponent } from './register-success/register-success.component';

export const routes: Routes = [
    {path: '', component: RegisterComponent},
    {path: 'successfulRegistration', component: RegisterSuccessComponent}

];
