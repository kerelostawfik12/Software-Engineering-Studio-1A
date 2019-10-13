import {Component, Inject, OnInit, ViewEncapsulation} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Notifications} from "../notifications";

@Component({
  selector: 'app-item-view',
  templateUrl: './item-view.component.html',
  styleUrls: ['./item-view.component.css'],
  encapsulation: ViewEncapsulation.None  // Use to disable CSS Encapsulation for this component
})
export class ItemViewComponent implements OnInit {
  private httpClient: HttpClient;
  private baseUrl: string;
  viewSetting: number = 0;
  items: Item[];
  array: number[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.refreshItems();
  }

  ngOnInit() {
  }

  refreshItems() {
    this.httpClient.get<Item[]>(this.baseUrl + 'api/Item/AllItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }

  delete(item: Item) {
    const message = `You are about to delete:\n${item.name}\n\nAre you sure?`;
    if (!confirm(message)) {
      return;
    }
    this.httpClient.post(this.baseUrl + 'api/Item/RemoveItem', {value: item.id}).subscribe(result => {
      this.refreshItems();
      Notifications.success("Successfully removed " + item.name + ".");
    }, error => {
      console.error(error);
      Notifications.error("Could not delete the item.");
    });
  }

}
