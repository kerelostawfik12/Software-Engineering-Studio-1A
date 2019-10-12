import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {Title} from "@angular/platform-browser";
import {Notifications} from "../notifications";

@Component({
  selector: 'app-thank-you',
  templateUrl: './thank-you.component.html',
  styleUrls: ['./thank-you.component.css']
})
export class ThankYouComponent implements OnInit {
  public lootBoxItemIds: string[] = [];
  public lootBoxItems: Item[] = [];

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute, private title: Title) {
    this.title.setTitle("Thank you - NotAmazon.com");
    Notifications.success("Order placed successfully. Thank you.");
    let lootParam = this.route.snapshot.paramMap.get('loot');
    if (lootParam == null || lootParam.trim() == "")
      return;
    this.lootBoxItemIds = this.route.snapshot.paramMap.get('loot').split(",");
    this.updateLootBoxItems();
  }

  ngOnInit() {
  }

  async updateLootBoxItems() {
    for (let i = 0; i < this.lootBoxItemIds.length; i++) {
      await this.delay(1500);
      await this.httpClient.get<Item>(this.baseUrl + 'api/Item/GetItem', {params: {id: this.lootBoxItemIds[i]}}).subscribe(result => {
        this.lootBoxItems.push(result);

      }, error => {
        const defaultItem: Item = {
          id: -1,
          name: "Empty Cardboard Box",
          description: "",
          seller: null,
          sellerId: -1,
          views: 0,
          price: 0,
          imageURL: ""
        };
        this.lootBoxItems.push(defaultItem);
      })
    }
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

}
