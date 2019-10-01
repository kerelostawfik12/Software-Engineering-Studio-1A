import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from '@angular/router';
import {Title} from "@angular/platform-browser";
import {CartService} from '../cart.service';

@Component({
  selector: 'app-item-page',
  templateUrl: './item-page.component.html',
  styleUrls: ['./item-page.component.css']
})
export class ItemPageComponent implements OnInit {
  item: Item;
  id: number;

  constructor(private http: HttpClient,
              @Inject('BASE_URL') private baseUrl: string,
              private route: ActivatedRoute,
              private cartService: CartService,
              private title: Title,
              private router: Router) {
    this.id = parseInt(this.route.snapshot.paramMap.get('id'));
    http.get<Item>(baseUrl + 'api/Item/GetItem', {params: {id: String(this.id)}}).subscribe(result => {
      this.item = result;
      this.title.setTitle(this.item.name + " - NotAmazon.com");
    }, error => console.error(error));
  }

  ngOnInit() { }

  addToCart() {
    this.cartService.addToCart(this.item);
  }
}
