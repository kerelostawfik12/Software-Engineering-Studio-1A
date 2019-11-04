import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {CartService} from "../cart.service";
import {User, UserService} from "../user.service";
import {Router} from "@angular/router";
import {Notifications} from "../notifications";
import * as assert from "assert";
import {FormatterService} from '../formatter.service';

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

  constructor(private httpClient: HttpClient, private router: Router, @Inject('BASE_URL') private baseUrl: string, private cartService: CartService, private userService: UserService, public formatter: FormatterService) {
    this.userService.getCurrent().subscribe(x => {
      this.user = x;
    });

  }

  ngOnInit() {
    let thisRef = this;
    paypal.Buttons({
      createOrder: function () {
        thisRef.cartService.refreshItems();
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
        Notifications.info("Your transaction is being processed. This may take a few seconds. Please wait...");
        return fetch(thisRef.baseUrl + 'api/Transaction/CapturePaypalOrder?orderId=' + data.orderID, {
          method: 'get'
        }).then(function (res) {
          return res.json();
        }).then(function (details) {
          try {
            assert(details.order.status == "COMPLETED");
            thisRef.cartService.refreshItems();
            if (details.lootBoxItems == null) {
              window.location.assign('/thank-you/');
            } else {
              let params = "";
              for (let i = 0; i < details.lootBoxItems.length; i++) {
                params += details.lootBoxItems[i];
                if (i != details.lootBoxItems.length - 1)
                  params += ","
              }
              window.location.assign('/thank-you/' + params);
            }
          } catch {
            thisRef.cartService.refreshItems();
            Notifications.error("An error occurred processing your transaction. Please check that you have not" +
              " been charged, and try again.");
          }

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
