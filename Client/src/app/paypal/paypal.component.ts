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
    paypal.Buttons().render('#buttons',);

    // paypal.configure({
    //  'mode' : 'Sandbox', //live or sandbox
    //  'client_id' : 'AbY5qhTQK2zo_163bAA1xkA19YkBMg8uiLqqh3MIAULsH2yF7T2ZGQ38cx740JmcUq24_bwNLS6-e3LF',
    //  'client_secret' : 'EMicuENMd4PD-W86tF2wEvUFHTCf_bHhNqJYwc7zsRvEnKKj1tmSDlv30H220hxyeq-f0rbevVAJgGrk'


  }




}
