import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-item-view',
  templateUrl: './item-view.component.html',
  styleUrls: ['./item-view.component.css']
})
export class ItemViewComponent implements OnInit {
  viewSetting: number = 0;
  items: Item[];
  item: Item;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Item[]>(baseUrl + 'api/Data/AllItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }

  ngOnInit() {
  }

  switchView() {
    if (this.viewSetting === 0) {
      this.viewSetting = 1;
    }
    else {
      this.viewSetting = 0;
    }
  }

}
