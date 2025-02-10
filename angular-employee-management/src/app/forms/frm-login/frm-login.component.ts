import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth-services/auth-service.service';
import { AuthenticatedUserModel } from '../../models/authenticated-user-model';

@Component({
  selector: 'app-frm-login',
  templateUrl: './frm-login.component.html',
  styleUrls: ['./frm-login.component.css'],
  imports: [CommonModule, FormsModule, RouterModule]
})
export class FrmLoginComponent implements OnInit {
  user: AuthenticatedUserModel | null = null;
  email: string = '';
  password: string = '';

  // constructor(private router: Router) { }

  ngOnInit() {
    // check if authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/main']);
    }
  }

  // login() {
  //   console.log(this.email, this.password);
  //   this.router.navigate(['/main']);
  // }
  constructor(private authService: AuthService,
    private router: Router) { }

  login() {
    this.authService.login(this.email, this.password)
      .subscribe();
  }
}
