import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}

  constructor(public accountService: AccountService,private router:Router,private toastr:ToastrService) {

  }
  ngOnInit(): void {
  }


  login() {
    // this is an http request => no need to unsubscribe because http requests completes
    this.accountService.login(this.model).subscribe({
      next: _ => this.router.navigateByUrl('/members')   
     })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
