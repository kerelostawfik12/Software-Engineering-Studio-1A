import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-recommended-items',
  templateUrl: './recommended-items.component.html',
  styleUrls: ['./recommended-items.component.css']
})
export class RecommendedItemsComponent implements OnInit {
  private httpClient: HttpClient;
  private baseUrl: string;
  items: Item[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.refreshItems();
  }

  ngOnInit() {
  }

  refreshItems() {
    this.httpClient.get<Item[]>(this.baseUrl + 'api/Item/RecommendedItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }
}
