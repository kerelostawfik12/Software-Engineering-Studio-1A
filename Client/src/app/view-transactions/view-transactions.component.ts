import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-view-transactions',
  templateUrl: './view-transactions.component.html',
  styleUrls: ['./view-transactions.component.css']
})
export class ViewTransactionsComponent implements OnInit {

  viewSetting: number = 0;
  items: TransactionItem[];
  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.refreshItems();
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
