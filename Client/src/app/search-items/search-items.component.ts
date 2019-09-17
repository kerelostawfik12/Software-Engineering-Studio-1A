import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {NavbarComponent} from "../navbar/navbar.component";

@Component({
  selector: 'app-browse-items',
  templateUrl: './search-items.component.html',
  styleUrls: ['./search-items.component.css']
})

// TODO: Pages
export class SearchItemsComponent implements OnInit {
  query: string = '';
  searchResult: SearchItemResult = null;
  router: Router;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute) {
    this.query = this.route.snapshot.paramMap.get('query');
    if (this.query == null)
      this.query = "";
    this.router = router;
    http.get<SearchItemResult>(baseUrl + 'api/Item/SearchItems', {params: {query: this.query}}).subscribe(result => {
      this.searchResult = result;
    }, error => console.error(error));
  }

  ngOnInit() {
    NavbarComponent.setSearchText(this.query.split('?')[0]);
  }

}
