import {Component, Inject, OnInit} from '@angular/core';
import {User, UserService} from "../user.service";
import {HttpClient} from "@angular/common/http";


@Component({
  selector: 'app-view-customers',
  templateUrl: './view-customers.component.html',
  styleUrls: ['./view-customers.component.css']
})
export class ViewCustomersComponent implements OnInit {
  private httpClient: HttpClient;
  private baseUrl: string;
  private user: User;
  customers: Customer[];

  constructor(
    @Inject('BASE_URL') baseUrl: string,
    private userService: UserService,
    http: HttpClient,) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.getCustomers();
  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(x => this.user = x);
  }

  getCustomers(){
    this.httpClient.get<Customer[]>(this.baseUrl + 'api/Account/GetAllCustomers').subscribe(result => {
      this.customers = result;
    }, error => {console.error(error);
    });
  }


}
