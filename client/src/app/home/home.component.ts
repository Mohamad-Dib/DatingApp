import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

users:any;


registerMode=false;

  ngOnInit(): void {
    this.getUsers();
}

constructor(private http:HttpClient) {
    
}

cancelRegisterMode(event:boolean)
{
  this.registerMode = event;
}

registerToggle(){
  this.registerMode = !this.registerMode;
}

getUsers(){
  this.http.get("https://localhost:5001/api/users").subscribe({
    next: response => this.users = response,
    error: error => console.log(error),
    complete: () => console.log("Request Completed")
  });
}
}
