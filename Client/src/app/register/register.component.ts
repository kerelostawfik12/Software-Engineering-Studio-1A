import {Component, Inject} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormBuilder} from '@angular/forms'
import {HttpClient} from "@angular/common/http";
import {Notifications} from "../notifications";
import {User} from "../user";


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
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

  public onSubmit() {
    if (User.current == null) {
      Notifications.error("Please try again later.");
      return;
    }
    if (User.current.isLoggedIn) {
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
      Notifications.success("Successfully created account.");
      window.location.reload();
    }, error => {
      Notifications.error(error);
    });
  }


}
