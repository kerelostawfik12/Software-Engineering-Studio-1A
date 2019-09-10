import {Component, Inject} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormBuilder} from '@angular/forms'
import {HttpClient} from "@angular/common/http";


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
    /*this.registerForm = this.formBuilder.group({
      firstName: '',
      lastName: '',
      password: '',
      emailAddress: '',
    });
    */

  }

  public onSubmit() {
    const customerAccountForm: CustomerAccountForm = {
      firstName: (document.getElementById("firstName") as HTMLInputElement).value,
      lastName: (document.getElementById("lastName") as HTMLInputElement).value,
      password: (document.getElementById("password") as HTMLInputElement).value,
      email: (document.getElementById("email") as HTMLInputElement).value,
    };
    this.httpClient.post(this.baseUrl + 'api/Account/CreateCustomerAccount', customerAccountForm).subscribe();
    window.alert("Look at the Console (Right Click -> inspect)");
  }


}
