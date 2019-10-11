import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {CartService} from "../cart.service";
import {User, UserService} from "../user.service";
import {Router} from "@angular/router";

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

  constructor(private httpClient: HttpClient, private router: Router, @Inject('BASE_URL') private baseUrl: string, private cartService: CartService, private userService: UserService) {
    this.userService.getCurrent().subscribe(x => {
      this.user = x;
    });

  }

  ngOnInit() {
    let thisRef = this;
    paypal.Buttons({
      createOrder: function () {
        return fetch(thisRef.baseUrl + 'api/Transaction/CreatePaypalOrder', {
          method: 'post',
          headers: {
            'content-type': 'application/json'
          }
        }).then(function (res) {
          return res.json();
        }).then(function (data) {
          return data.id; // Use the same key name for order ID on the client and server
        });
      },
      onApprove: function (data) {
        return fetch(thisRef.baseUrl + 'api/Transaction/CapturePaypalOrder?orderId=' + data.orderID, {
          method: 'get'
        }).then(function (res) {
          return res.json();
        }).then(function (details) {
          thisRef.router.navigateByUrl('/thank-you');
        })
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
