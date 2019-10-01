import {Component, Input, OnInit} from '@angular/core';
import {CartService} from '../cart.service';

@Component({
  selector: 'app-cart-item',
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.css']
})
export class CartItemComponent implements OnInit {
  @Input() public item: Item;

  constructor(private cartService: CartService) {
  }

  ngOnInit() {
  }

  remove() {
    this.cartService.removeItem(this.item);
  }

}
