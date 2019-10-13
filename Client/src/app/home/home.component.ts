import {Component} from '@angular/core';
//import Swipe from 'swipejs/build/swipe.min.js';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  ngOnInit() {
    // @ts-ignore
    (<any>window).mySwipe = new Swipe(document.getElementById('slider'), {
      startSlide: 0,
      auto: 3000,
      draggable: true,
      autoRestart: true,
      continuous: true,
      disableScroll: true,
      stopPropagation: true,

    });
  }

}
