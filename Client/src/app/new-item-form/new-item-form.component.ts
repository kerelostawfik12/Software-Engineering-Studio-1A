import {Component, Inject} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Notifications} from "../notifications";


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

    // Don't send it to the server if it's invalid
    if (!this.isValid(newItemForm))
      return;

    this.httpClient.post(this.baseUrl + 'api/Item/CreateItem', newItemForm).subscribe(result => {
      const item: Item = result as Item;
      this.router.navigateByUrl('/item/' + item.id);
      Notifications.success("Successfully created " + item.name + ".");
    }, error => {
      console.error(error);
      Notifications.error(error);
    });
  }

  // Returns whether or not the form as a whole is valid, whilst showing the appropriate warning message.
  private isValid(newItemForm: NewItemForm): boolean {
    if (newItemForm.name.length > 200) {
      Notifications.warning("Item names are limited to 200 characters.");
      return false;
    }
    if (newItemForm.name.length < 1) {
      Notifications.warning("Items must have a valid name.");
      return false;
    }
    if (newItemForm.description.length < 1) {
      Notifications.warning("Items must have a valid description.");
      return false;
    }
    if (isNaN(parseFloat(newItemForm.price.replace('$', '')))) {
      Notifications.warning("Items must have a valid price.");
      return false;
    }

    return true;
  }

  ngOnInit() {
  }

}
