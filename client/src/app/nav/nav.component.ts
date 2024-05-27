import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}

  constructor(public accountService: AccountService) {

  }
  ngOnInit(): void {
  }


  login() {
    // this is an http request => no need to unsubscribe because http requests completes
    this.accountService.login(this.model).subscribe({
      next: respone => {
        console.log(respone);
      },
      error: error => console.log(error)


    })
  }

  logout() {
    this.accountService.logout();
  }
}
