import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  private httpClient : HttpClient;
  private baseUrl : string;
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    document.getElementById('search-button').ondragstart = function() { return false; };
  }


  search() {
    alert("bepis");
    const bepis : CustomerAccountForm = {email:"bepis123@bepismail.com" + 100 * Math.random(), firstName: "joseph", lastName: "smith", password: "qwertyPassword123"};
    this.httpClient.post(this.baseUrl + 'api/Account/CreateAccountForm', bepis).subscribe();
  }

}
