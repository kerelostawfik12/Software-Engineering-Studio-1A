import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {CartService} from "../cart.service";


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {

  public items : Item[];

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string, private cartService: CartService) {


  }

  ngOnInit() {
    this.cartService.refreshItems();
    this.items = [];
    this.cartService.getItems().subscribe(items => {
      this.items = items;
      console.log(items);
    });

  }

}
