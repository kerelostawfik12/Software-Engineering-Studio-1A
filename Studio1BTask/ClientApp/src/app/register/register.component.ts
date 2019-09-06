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
  private http: HttpClient;
  private baseUrl: string;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    @Inject('BASE_URL') baseUrl: string
  ) {
    /*this.registerForm = this.formBuilder.group({
      firstName: '',
      lastName: '',
      password: '',
      emailAddress: '',
    });
    */

  }

  onSubmit() {
    const customerAccountForm: CustomerAccountForm = {
      firstName: (document.getElementById("firstName") as HTMLInputElement).value,
      lastName: (document.getElementById("lastName") as HTMLInputElement).value,
      password: (document.getElementById("password") as HTMLInputElement).value,
      email: (document.getElementById("email") as HTMLInputElement).value,
    };
    this.http.post(this.baseUrl + 'api/Account/CreateAccountForm', customerAccountForm).subscribe();
    window.alert("Look at the Console (Right Click -> inspect)");
  }


}
