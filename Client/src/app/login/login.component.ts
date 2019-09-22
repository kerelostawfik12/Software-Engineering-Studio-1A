import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {FormBuilder} from "@angular/forms";
import {Notifications} from "../notifications";
import {User, UserService} from "../user.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  private httpClient: HttpClient;
  private baseUrl: string;
  private user: User;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private router: Router,
    http: HttpClient,
    private userService: UserService,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.httpClient = http;
    this.baseUrl = baseUrl;

  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(x => this.user = x);
  }

  onSubmit() {
    if (this.user.isLoggedIn) {
      Notifications.error("Please log out before logging in again.");
      return;
    }
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
