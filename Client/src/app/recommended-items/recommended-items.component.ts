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
  mostViewed: Item[] = [];
  topSellers: Item[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.refreshItems();
  }

  ngOnInit() {
  }

  private static addToArrayWithDelay(inputArray: any[], outputArray: any[], delay: number, delayEase: number = 1, index: number = 0) {
    if (index >= inputArray.length)
      return;
    outputArray.push(inputArray[index]);
    setTimeout(function () {
      RecommendedItemsComponent.addToArrayWithDelay(inputArray, outputArray, delay * delayEase, delayEase, index + 1);
    }, delay * 1000);
  }

  refreshItems() {
    this.httpClient.get<Item[]>(this.baseUrl + 'api/Item/FrontPageItems').subscribe(result => {
      RecommendedItemsComponent.addToArrayWithDelay(result["mostViewed"], this.mostViewed, 0.3, 0.9);
      RecommendedItemsComponent.addToArrayWithDelay(result["topSellers"], this.topSellers, 0.3, 0.9);
    }, error => console.error(error));
  }
}
