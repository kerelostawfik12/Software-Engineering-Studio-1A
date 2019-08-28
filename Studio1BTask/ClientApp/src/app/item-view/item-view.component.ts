import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-item-view',
  templateUrl: './item-view.component.html',
  styleUrls: ['./item-view.component.css']
})
export class ItemViewComponent implements OnInit {
  viewSetting: number = 0;

  constructor() { }

  ngOnInit() {
  }

  switchView() {
    if (this.viewSetting === 0) {
      this.viewSetting = 1;
    }
    else {
      this.viewSetting = 0;
    }
  }

}
