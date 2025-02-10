import { Routes } from '@angular/router';
import { FrmLoginComponent } from './forms/frm-login/frm-login.component';
import { FrmMainComponent } from './forms/frm-main/frm-main.component';
import { FrmAboutComponent } from './forms/frm-about/frm-about.component';
import { FrmEmployeeListComponent } from './forms/frm-employee-list/frm-employee-list.component';
import { authGuard } from './services/auth-guards/auth-guard';
import { FrmEmployeeEditComponent } from './forms/frm-employee-edit/frm-employee-edit.component';

export const routes: Routes = [
    { path: 'login', component: FrmLoginComponent },
    { path: 'main', component: FrmMainComponent, canActivate: [authGuard] },
    { path: 'about', component: FrmAboutComponent, canActivate: [authGuard] },
    { path: 'employees', component: FrmEmployeeListComponent, canActivate: [authGuard] },
    { path: 'employees/edit/:id', component: FrmEmployeeEditComponent, canActivate: [authGuard] },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: '**', redirectTo: 'login' }
];
