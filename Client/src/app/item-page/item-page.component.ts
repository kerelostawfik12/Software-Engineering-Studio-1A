import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-item-page',
  templateUrl: './item-page.component.html',
  styleUrls: ['./item-page.component.css']
})
export class ItemPageComponent implements OnInit {
  item: Item;
  id: number;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute) {
  this.id = parseInt(this.route.snapshot.paramMap.get('id'));
    http.get<Item>(baseUrl + 'api/Item/GetItem', {params: {id: String(this.id)}}).subscribe(result => {
      this.item = result;
    }, error => console.error(error));
  }

  ngOnInit() { }

  add(item: Item) {

  }

}
