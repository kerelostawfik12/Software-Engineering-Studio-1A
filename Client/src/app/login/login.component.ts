import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {FormBuilder} from "@angular/forms";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.httpClient = http;
    this.baseUrl = baseUrl;

  }

  ngOnInit() {
  }

  onSubmit() {
    const loginForm = {
      "email" : (document.getElementById("email") as HTMLInputElement).value,
      "password": (document.getElementById("password") as HTMLInputElement).value
    };
    this.httpClient.post(this.baseUrl + 'api/Account/Login', loginForm).subscribe();
  }
}
