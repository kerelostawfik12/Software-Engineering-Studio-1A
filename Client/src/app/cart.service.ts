import {Inject, Injectable} from '@angular/core';
import {Notifications} from "./notifications";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CartService {
  public isLoaded: boolean = false;
  private items: BehaviorSubject<Item[]> = new BehaviorSubject<Item[]>(new Array<Item>());

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.refreshItems();
  }

  public addToCart(item: Item) {
    this.httpClient.post(this.baseUrl + 'api/Item/AddItemToCart', {"value": item.id}).subscribe(result => {
      Notifications.success("Added " + item.name + " to cart.");
      // Set items in cart to whatever the response from the server now says is in the cart
      this.items.next(result as Item[]);
    }, error => {
      console.error(error);
      Notifications.error("Could not add item to cart. Please refresh the page, and try again.");
    });
  }

  public refreshItems(): Observable<Item[]> {
    this.httpClient.get<Item[]>(this.baseUrl + 'api/Item/GetItemsInCart').subscribe(result => {
      this.isLoaded = true;
      this.items.next(result as Item[]);
    }, error => {
      console.log(error);
      // Quick workaround for incorrect cart data on first page load
      this.isLoaded = true;
      this.items.next(new Array<Item>());
    });
    return this.items;
  }

  public getItems(): Observable<Item[]> {
    return this.items;
  }

  public removeItem(item: Item): void {
    this.httpClient.post(this.baseUrl + 'api/Item/RemoveItemFromCart', {"value": item.id}).subscribe(result => {
      Notifications.success("Removed " + item.name + " from cart.");
      // Set items in cart to whatever the response from the server now says is in the cart
      this.items.next(result as Item[]);
    }, error => {
      console.error(error);
      Notifications.error(error);
    });
  }
}
