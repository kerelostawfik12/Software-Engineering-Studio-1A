import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {FormBuilder} from "@angular/forms";
import {Notifications} from "../notifications";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  private httpClient: HttpClient;
  private baseUrl: string;
  formGroup: any;

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
    document.body.style.cursor = "progress";
    this.httpClient.post(this.baseUrl + 'api/Account/Login', loginForm).subscribe(result => {
      const success = result as boolean;
      if (success) {
        Notifications.success("Logging in...");
        window.location.assign('/');
      } else {
        Notifications.error("Invalid credentials.");
        document.body.style.cursor = "default";
      }
    });
  }
}
