import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-item-page',
  templateUrl: './item-page.component.html',
  styleUrls: ['./item-page.component.css']
})
export class ItemPageComponent implements OnInit {
  items: Item[];
  item: Item;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Item[]>(baseUrl + 'api/Data/AllItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }

  ngOnInit() {
  }

}
