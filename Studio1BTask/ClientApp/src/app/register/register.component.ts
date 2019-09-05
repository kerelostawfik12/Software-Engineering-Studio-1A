import {Component} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormBuilder} from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
  ) {
    this.registerForm = this.formBuilder.group({
      firstName: '',
      lastName: '',
      address: '',
      emailAddress: '',
      confirmEmail: '',
    });
  }

  onSubmit(userData) {
    console.warn('Sign up was Successful!', userData);
    window.alert("Look at the Console (Right Click -> inspect)");
    this.registerForm.reset();
  }


}
