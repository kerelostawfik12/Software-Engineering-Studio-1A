import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User, UserService} from "../user.service";
import {Router} from "@angular/router";
import {FormBuilder} from "@angular/forms";
import {Notifications} from "../notifications";

@Component({
  selector: 'app-admin-register',
  templateUrl: './admin-register.component.html',
  styleUrls: ['./admin-register.component.css']
})
export class AdminRegisterComponent implements OnInit {

  private httpClient: HttpClient;
  private baseUrl: string;
  private user: User;


  constructor(private route: Router,
              @Inject('BASE_URL') baseUrl: string,
              private formBuilder: FormBuilder,
              private userService: UserService,
              http: HttpClient,
  ) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(x => this.user = x);
  }

  onSubmit(){
    if (this.user == null){
      Notifications.error('Please try again later.');
      return;
    }

    const adminAccountForm: AdminAccountForm = {
      email: (document.getElementById("email") as HTMLInputElement).value,
      password: (document.getElementById("password") as HTMLInputElement).value,
    };
    this.httpClient.post(this.baseUrl + 'api/Account/CreateAdminAccount', adminAccountForm).subscribe(result => {
      console.log(result);
      if (result["error"] != null || !result["success"]){
        Notifications.error(result["error"]);
        return;
      }
      Notifications.success("Administrator was successfully created");
    }, error => {
      Notifications.error(error);
    });

  }
}
