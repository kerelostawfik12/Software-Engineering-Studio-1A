import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User, UserService} from "../user.service";
import {FormatterService} from '../formatter.service';

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

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private userService: UserService, private formatter: FormatterService) {
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
    }, error => console.error(error));
  }

  getDateString(date: Date): string {
    // New date object needs to be created, as typescript for some reason does not accept the one in the interface
    let d = new Date(date);
    return this.addZero(d.getDate()) + "/" + this.addZero(d.getMonth() + 1) + "/" + d.getFullYear();
  }

  getTimeString(date: Date): string {
    let d = new Date(date);
    return this.addZero(d.getHours()) + ":" + this.addZero(d.getMinutes()) + ":" + this.addZero(d.getSeconds());
  }

  addZero(input: any): string {
    let str = input.toString();
    if (str.length == 1)
      return "0" + str;
    if (str.length == 0)
      return "00";
    return str;
  }


}
