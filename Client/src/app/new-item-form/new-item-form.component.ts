import {Component, Inject} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  selector: 'app-new-item-form',
  templateUrl: './new-item-form.component.html',
  styleUrls: ['./new-item-form.component.css']
})
export class NewItemFormComponent {

  private httpClient: HttpClient;
  private baseUrl: string;
  private router: Router;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.router = router;
  }

  public submit() {
    const newItemForm: NewItemForm = {
      name: (document.getElementById("name") as HTMLInputElement).value,
      description: (document.getElementById("description") as HTMLInputElement).value,
      price: (document.getElementById("price") as HTMLInputElement).value,
    };
    this.httpClient.post(this.baseUrl + 'api/Item/CreateItem', newItemForm).subscribe(result => {
      const newItemId = (result as Item).id;
      this.router.navigateByUrl('/item/' + newItemId);
    }, error => console.error(error));
  }


  ngOnInit() {
  }

}
