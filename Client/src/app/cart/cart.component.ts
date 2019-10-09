import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {CartService} from "../cart.service";
import {User, UserService} from "../user.service";

declare var paypal: any;


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  public user: User;
  public items: Item[];
  public totalPrice: number;

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string, private cartService: CartService, private userService: UserService) {
    this.userService.getCurrent().subscribe(x => {
      this.user = x;
    });

  }

  ngOnInit() {
    let thisRef = this;
    paypal.Buttons({
      createOrder: function (data, actions) {
        return actions.order.create({
          purchase_units: [{
            amount: {
              value: thisRef.getTotalPrice(),
              currency_code: 'AUD'
            }
          }
          ]
        });
      },
      onApprove: function (data, actions) {
        return null;
      }
    }).render('#buttons');
    this.cartService.refreshItems();
    this.items = [];
    this.cartService.getItems().subscribe(items => {
      this.items = items;
      this.totalPrice = this.getTotalPrice();
    });

  }

  getTotalPrice(): number  {
    let price: number = 0;
    for (let item of this.items) {
      price += item.price;
    }
    return price;
  }

  remove(item : Item) {
    this.cartService.removeItem(item);

  }


}
