import {Component, Inject, OnInit} from '@angular/core';
import {User, UserService} from "../user.service";
import {HttpClient} from "@angular/common/http";
import {Notifications} from "../notifications";


@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css']
})
export class ProfilePageComponent implements OnInit{
  public user: User;

  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(
    private userService: UserService,
    @Inject('BASE_URL') baseUrl: string,
    http: HttpClient,
  ) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(x => this.user = x);
  }


  changeName() {
    this.httpClient.post(this.baseUrl + 'api/Account/ChangeName', {
      firstName: (document.getElementById("firstName") as HTMLInputElement).value,
      lastName: (document.getElementById("lastName") as HTMLInputElement).value,
      name: (document.getElementById("name") as HTMLInputElement).value,
    }).subscribe(result => {
      Notifications.success("Successfully changed name.");
      window.location.assign('/');
    }, error => {
      console.error(error);
      if (error["error"] != null)
        Notifications.error(error["error"]["error"]);
      else
        Notifications.error("An unknown error occured.");
    })
  }

  changeEmail() {
    this.httpClient.post(this.baseUrl + 'api/Account/ChangeEmail', {
      oldEmail: (document.getElementById("oldEmail") as HTMLInputElement).value,
      newEmail: (document.getElementById("newEmail") as HTMLInputElement).value,
      password: (document.getElementById("password") as HTMLInputElement).value
    }).subscribe(result => {
      Notifications.success("Successfully changed email address.");
      window.location.assign('/login');
    }, error => {
      console.error(error);
      if (error["error"] != null)
        Notifications.error(error["error"]["error"]);
      else
        Notifications.error("An unknown error occured.");
    })
  }

  changePassword() {
    this.httpClient.post(this.baseUrl + 'api/Account/ChangePassword', {
      email: (document.getElementById("email") as HTMLInputElement).value,
      oldPassword: (document.getElementById("oldPassword") as HTMLInputElement).value,
      newPassword: (document.getElementById("newPassword") as HTMLInputElement).value
    }).subscribe(result => {
      Notifications.success("Successfully changed password.");
      window.location.assign('/login');
    }, error => {
      console.error(error);
      if (error["error"] != null)
        Notifications.error(error["error"]["error"]);
      else
        Notifications.error("An unknown error occured.");
    })
  }
}
