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
  mostViewed: Item[];
  topSellers: Item[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.refreshItems();
  }

  ngOnInit() {
  }

  refreshItems() {
    this.httpClient.get<Item[]>(this.baseUrl + 'api/Item/FrontPageItems').subscribe(result => {
      this.mostViewed = result["mostViewed"];
      this.topSellers = result["topSellers"];
    }, error => console.error(error));
  }
}
