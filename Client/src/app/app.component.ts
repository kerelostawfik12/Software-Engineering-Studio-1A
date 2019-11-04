import {Component, Inject, NgZone, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Notifications} from "./notifications";
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";
import {UserService} from "./user.service"


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

})
export class AppComponent implements OnInit {
  title = 'app';

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private route: ActivatedRoute,
    private zone: NgZone,
    private userService: UserService) {
    // Get new session token and fill in local user object with response
    http.get(baseUrl + 'api/Account/CreateSession', {}).subscribe(result => {
      if (result === true) {
        console.log("Created new session.");
        userService.setCurrent(result, true, false);
      } else {
        console.log("Using existing session.");
        console.log(result);
        let isLoggedIn = result != null && result != false;
        userService.setCurrent(result, true, isLoggedIn);
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
