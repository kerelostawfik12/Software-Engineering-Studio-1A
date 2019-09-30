import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder} from '@angular/forms'
import {HttpClient} from "@angular/common/http";
import {User, UserService} from "../user.service";
import {ActivatedRoute, Router} from '@angular/router';
import {Notifications} from "../notifications";


@Component({
  selector: 'app-seller-register',
  templateUrl: './seller-register.component.html',
  styleUrls: ['./seller-register.component.css']
})
export class SellerRegisterComponent implements OnInit {
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

    const sellerAccountForm: SellerAccountForm = {
      Name: (document.getElementById("name") as HTMLInputElement).value,
      Email: (document.getElementById("email") as HTMLInputElement).value,
      Password: (document.getElementById("password") as HTMLInputElement).value,
    };
    this.httpClient.post(this.baseUrl + 'api/Account/CreateSellerAccount', sellerAccountForm).subscribe(result => {
      console.log(result);
      if (result["error"] != null || !result["success"]){
        Notifications.error(result["error"]);
        return;
      }
      Notifications.success("Seller was successfully created");
    }, error => {
      Notifications.error(error);
    });

  }
}
