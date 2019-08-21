import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {

  public items : Item[];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    http.get<Item[]>(baseUrl + 'api/Data/AllItems').subscribe(result => {
      this.items = result;
    }, error => console.error(error));
  }

  ngOnInit() {

  }



}
