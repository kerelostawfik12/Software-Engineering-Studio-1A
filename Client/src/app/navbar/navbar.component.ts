import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  private static instance: NavbarComponent;
  private httpClient : HttpClient;
  private baseUrl : string;
  private router: Router;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.router = router;
  }

  public static setSearchText(text: string) {
    (document.getElementById('search-input') as HTMLInputElement).value = text;
  }

  ngOnInit() {
    NavbarComponent.instance = this;
    document.getElementById('search-button').ondragstart = function() { return false; };
  }

  search() {
    const searchInput = document.getElementById('search-input') as HTMLInputElement;
    if (searchInput.value == null) {
      searchInput.value = "";
    }
    const stripSymbolsRegex = /[-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g;
    searchInput.value = searchInput.value.replace(stripSymbolsRegex, '');
    searchInput.value = searchInput.value.replace('bepis', 'bepisbepis');
    searchInput.value = searchInput.value.replace('jamie!', 'bepis');
    this.router.navigateByUrl('/search/' + searchInput.value);
  }

}
