import {Component} from '@angular/core';
import {User, UserService} from "../user.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  user: User = null;
  constructor(private userService: UserService){
  }

  ngOnInit() {
    this.userService.getCurrent().subscribe(result => {
      this.user = result;
    })
  }

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
