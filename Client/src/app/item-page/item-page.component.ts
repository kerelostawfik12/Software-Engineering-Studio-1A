import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from '@angular/router';
import {Title} from "@angular/platform-browser";
import {CartService} from '../cart.service';
import {Notifications} from "../notifications";

@Component({
  selector: 'app-item-page',
  templateUrl: './item-page.component.html',
  styleUrls: ['./item-page.component.css']
})
export class ItemPageComponent implements OnInit {
  public item: Item;
  public boughtTogether: Item[] = [];
  public id: number;
  public errorText: string = "";

  constructor(private http: HttpClient,
              @Inject('BASE_URL') private baseUrl: string,
              private route: ActivatedRoute,
              private cartService: CartService,
              private title: Title,
              private router: Router) {
    this.id = parseInt(this.route.snapshot.paramMap.get('id'));
    http.get(baseUrl + 'api/Item/GetItemPage', {params: {id: String(this.id)}}).subscribe(result => {
      this.item = result["item"] as Item;
      this.boughtTogether = result["boughtTogether"] as Item[];
      this.title.setTitle(this.item.name + " - NotAmazon.com");
    }, error => {
      this.errorText = "This item does not exist, or has been removed.";
      Notifications.error(this.errorText);
      console.error(error)
    });
  }

  ngOnInit() { }

  addToCart() {
    this.cartService.addToCart(this.item);
  }
}
