import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User, UserService} from "../user.service";

@Component({
  selector: 'app-view-transactions',
  templateUrl: './view-transactions.component.html',
  styleUrls: ['./view-transactions.component.css']
})
export class ViewTransactionsComponent implements OnInit {
  user: User;
  items: TransactionItem[];

  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private userService: UserService) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.userService.getCurrent().subscribe(result => {
      this.user = result;
      this.refreshItems();
    });
  }

  ngOnInit() {
  }

  refreshItems() {

    this.httpClient.get<TransactionItem[]>(this.baseUrl + 'api/Transaction/AllTransactions').subscribe(result => {
      this.items = result;
      console.log(this.items);
    }, error => console.error(error));
  }

}
