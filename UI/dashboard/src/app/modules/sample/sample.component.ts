import { Component, OnInit } from '@angular/core';
import 'bcc-pay/dist/bcc-pay';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sample',
  templateUrl: './sample.component.html',
  styleUrls: ['./sample.component.scss'],
})
export class SampleComponent implements OnInit {
  serverUrl: string;
  netsCheckoutKey: string;
  constructor() {
    this.serverUrl = environment.bccPayUrl;
    this.netsCheckoutKey = environment.netsCheckoutKey;
  }

  ngOnInit(): void {}
}
