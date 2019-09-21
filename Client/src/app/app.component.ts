import {Component, Inject, NgZone, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Notifications} from "./notifications";
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";
import {User} from "./user";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

})
export class AppComponent implements OnInit {
  title = 'app';
  router: Router;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute, private zone: NgZone) {
    this.router = router;

    // Get new session token
    http.get(baseUrl + 'api/Account/CreateSession', {}).subscribe(result => {
      if (result === true) {
        console.log("Created new session.");
        let user = new User();
        user.data = result;
        user.isLoaded = true;
        user.isLoggedIn = false;
        this.zone.run(() => {
          User.current = user;
        })
      } else {
        console.log("Using existing session.");
        console.log(result);

        let user = new User();
        user.data = result;
        user.isLoaded = true;
        user.isLoggedIn = result != null && result != false;
        this.zone.run(() => {
          User.current = user;
        })
      }
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
