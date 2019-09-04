import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from '@angular/router';


@Component({
  selector: 'app-item-page',
  templateUrl: './item-page.component.html',
  styleUrls: ['./item-page.component.css']
})
export class ItemPageComponent implements OnInit {
  items: Item[];
  item: Item;
  id: number;

  constructor(
    http: HttpClient, @Inject('BASE_URL') baseUrl: string,
    private route: ActivatedRoute) {
    http.get<Item[]>(baseUrl + 'api/Data/AllItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }

  ngOnInit() {
    this.id = +this.route.snapshot.paramMap.get('id');
  }
}
