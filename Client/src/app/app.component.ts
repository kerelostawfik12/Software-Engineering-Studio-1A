import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Notifications} from "./notifications";
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

})
export class AppComponent implements OnInit {
  title = 'app';
  router: Router;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute) {
    this.router = router;
    // Warm up database
    http.get<Item>(baseUrl + 'api/Item/GetItem', {params: {id: '1'}}).subscribe(result => {
      console.log("Warmed up database: \n" + (result as Item).name)
    }, error => {
      console.error(error);
      if (error.status == 404)
        Notifications.error("Could not connect to server.");
      else
        Notifications.error(error);
    });
  }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };

    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.router.navigated = false;
        window.scrollTo(0, 0);
      }
    });
  }
}
