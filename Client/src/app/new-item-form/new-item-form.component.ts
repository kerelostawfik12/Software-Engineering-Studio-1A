import {Component, ElementRef, Inject, ViewChild} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Notifications} from "../notifications";


@Component({
  selector: 'app-new-item-form',
  templateUrl: './new-item-form.component.html',
  styleUrls: ['./new-item-form.component.css']
})
export class NewItemFormComponent {
  @ViewChild('uploadButton') uploadButton: ElementRef;
  @ViewChild('submitButton') submitButton: ElementRef;
  private httpClient: HttpClient;
  public imageURL: string = "";
  private router: Router;
  private baseUrl: string = "";

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.router = router;
  }

  public setSubmissionEnabled(enabled: boolean) {
    (this.uploadButton.nativeElement as HTMLInputElement).disabled = !enabled;
    (this.submitButton.nativeElement as HTMLInputElement).disabled = !enabled;
  }

  public uploadImage() {
    this.setSubmissionEnabled(false);
    let imageFile = (document.getElementById("file") as HTMLInputElement).files.item(0);
    Notifications.info("Uploading your image, please wait... (" + Math.ceil(imageFile.size / 1000) + " KB)");

    this.httpClient.post(this.baseUrl + 'api/Item/UploadImage', imageFile).subscribe(result => {
      Notifications.success("Successfully uploaded image.");
      this.imageURL = result["url"];
      this.setSubmissionEnabled(true);
    }, error => {
      console.log(error);
      if (error.reason != null)
        Notifications.error(error.reason);
      else
        Notifications.error(
          "An error occurred uploading your file. Please ensure the file is compatible, and try again.");
      this.setSubmissionEnabled(true);
    });
  }
  public submit() {
    const newItemForm: NewItemForm = {
      name: (document.getElementById("name") as HTMLInputElement).value,
      description: (document.getElementById("description") as HTMLInputElement).value,
      price: (document.getElementById("price") as HTMLInputElement).value,
      imageURL: this.imageURL
    };

    // Don't send it to the server if it's invalid
    if (!this.isValid(newItemForm))
      return;

    // Ask for confirmation if an image hasn't been provided
    if (this.imageURL.trim() == "") {
      if (!confirm("Are you sure you want to submit this item without an image?"))
        return;
    }

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
