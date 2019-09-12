import {Component, Inject} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Notifications} from "./notifications";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

})
export class AppComponent {
  title = 'app';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
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
}
