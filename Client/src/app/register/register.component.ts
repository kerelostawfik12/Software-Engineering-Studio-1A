import {Component, Inject} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder} from '@angular/forms'
import {HttpClient} from "@angular/common/http";
import {Notifications} from "../notifications";
import {User, UserService} from "../user.service";


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
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

  public onSubmit() {
    if (this.user == null) {
      Notifications.error("Please try again later.");
      return;
    }
    if (this.user.isLoggedIn) {
      Notifications.error("Please log out before registering a new account.");
      return;
    }
    const customerAccountForm: CustomerAccountForm = {
      firstName: (document.getElementById("firstName") as HTMLInputElement).value,
      lastName: (document.getElementById("lastName") as HTMLInputElement).value,
      password: (document.getElementById("password") as HTMLInputElement).value,
      email: (document.getElementById("email") as HTMLInputElement).value,
    };
    this.httpClient.post(this.baseUrl + 'api/Account/CreateCustomerAccount', customerAccountForm).subscribe(result => {
      console.log(result);
      if (result["error"] != null || !result["success"]) {
        Notifications.error(result["error"]);
        return;
      }
      Notifications.success("Successfully created account.");
      window.location.assign('/');
    }, error => {
      Notifications.error(error);
    });
  }


}
