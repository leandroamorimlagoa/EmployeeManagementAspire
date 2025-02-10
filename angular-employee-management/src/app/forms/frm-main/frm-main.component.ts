import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-frm-main',
  templateUrl: './frm-main.component.html',
  styleUrls: ['./frm-main.component.css'],
  imports: [
    RouterModule,
    CommonModule
  ]
})
export class FrmMainComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  logout() {
    this.router.navigate(['/login']);
  }
}
