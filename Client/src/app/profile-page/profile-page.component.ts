import { Component, OnInit } from '@angular/core';
import {User, UserService} from "../user.service";


@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css']
})
export class ProfilePageComponent implements OnInit{

  constructor() { }

  ngOnInit() {
  }

}
