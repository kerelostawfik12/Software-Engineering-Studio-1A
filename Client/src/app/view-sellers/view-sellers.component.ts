import {Component, Inject, OnInit} from '@angular/core';
import {User, UserService} from "../user.service";
import {HttpClient} from "@angular/common/http";


@Component({
  selector: 'app-view-sellers',
  templateUrl: './view-sellers.component.html',
  styleUrls: ['./view-sellers.component.css']
})
export class ViewSellersComponent implements OnInit {
  private httpClient: HttpClient;
  private baseUrl: string;
  private user: User;
  sellers: Seller[];

  constructor(
    @Inject('BASE_URL') baseUrl: string,
    private userService: UserService,
    http: HttpClient,) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.getSellers();
  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(x => this.user = x);
  }

  getSellers(){
    this.httpClient.get<Seller[]>(this.baseUrl + 'api/Account/GetAllSellers').subscribe(result => {
      this.sellers = result;
    }, error => {console.error(error);
    });
}


}
