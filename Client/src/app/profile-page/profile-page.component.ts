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
  private httpClient: HttpClient;
  private baseUrl: string;
  private user: User;
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

   removeAccount(){
    this.httpClient.post(this.baseUrl + 'api/Account/RemoveAccount', {}).subscribe(result => {
      console.log(result);
      Notifications.success("Successfully removed " + User.name + "!");
    }, error => {
      console.error(error);
      Notifications.error("Account was not deleted")
    })
  }

}
