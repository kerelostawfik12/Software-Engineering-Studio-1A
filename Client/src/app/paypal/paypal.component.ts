import { Component, OnInit } from '@angular/core';

declare var paypal: any;

@Component({
  selector: 'app-paypal',
  templateUrl: './paypal.component.html',
  styleUrls: ['./paypal.component.css']
})
export class PaypalComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    paypal.Buttons().render('#buttons');
  }



}
