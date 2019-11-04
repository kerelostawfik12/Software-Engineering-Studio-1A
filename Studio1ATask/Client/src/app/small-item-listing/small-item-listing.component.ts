import {Component, Input, OnInit} from '@angular/core';
import {CartService} from '../cart.service';

@Component({
  selector: 'app-small-item-listing',
  templateUrl: './small-item-listing.component.html',
  styleUrls: ['./small-item-listing.component.css']
})
export class SmallItemListingComponent implements OnInit {

  @Input() public item: Item;
  @Input() public showAddToCart: boolean = true;

  constructor(private cartService: CartService) {
  }

  ngOnInit() {
  }

  getDollars(): number {
    return Math.floor(this.item.price);
  }

  getCents(): string {
    const centsValue: number = Math.floor(100 * (this.item.price - Math.floor(this.item.price)));
    if (centsValue < 1) {
      return ".00";
    }
    let cents = centsValue.toString();
    if (cents.length > 2)
      cents = cents.substring(0, 2);
    else if (cents.length < 2)
      cents = "0" + cents;
    cents = "." + cents;
    return cents;
  }

  addToCart() {
    this.cartService.addToCart(this.item);
  }
}
