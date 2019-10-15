import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {NavbarComponent} from "../navbar/navbar.component";
import {Title} from "@angular/platform-browser";

@Component({
  selector: 'app-browse-items',
  templateUrl: './search-items.component.html',
  styleUrls: ['./search-items.component.css']
})

// TODO: Pages
export class SearchItemsComponent implements OnInit {
  query: string = '';
  searchResult: SearchItemResult = null;

  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute, private titleService: Title) {
    this.refreshItems();
  }

  public refreshItems() {
    this.searchResult = null;
    this.query = this.route.snapshot.paramMap.get('query').trim();
    if (this.query == null)
      this.query = "";
    this.http.get<SearchItemResult>(this.baseUrl + 'api/Item/SearchItems', {params: {query: this.query}}).subscribe(result => {
      this.searchResult = result;
    }, error => console.error(error));
    this.titleService.setTitle(this.query + " - NotAmazon.com")
  }

  ngOnInit() {
    NavbarComponent.setSearchText(this.query.split('?')[0].trim());
  }

  sortByCheapest() {
    this.searchResult.items.sort((a, b) => a.price - b.price);
  }

  sortByMostExpensive() {
    this.searchResult.items.sort((a, b) => b.price - a.price);
  }

  sortByNewest() {
    this.searchResult.items.sort((a, b) => b.id - a.id);
  }

  sortByOldest() {
    this.searchResult.items.sort((a, b) => a.id - b.id);
  }
}
