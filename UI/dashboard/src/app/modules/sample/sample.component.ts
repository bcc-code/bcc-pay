import { Component } from '@angular/core';
import 'bcc-pay/dist/bcc-pay';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sample',
  templateUrl: './sample.component.html',
  styleUrls: ['./sample.component.scss'],
})
export class SampleComponent {
  server: string;
  netsCheckoutKey: string;
  requestHeaders: [{ key: string; value: string }];

  constructor() {
    this.server = environment.bccPayUrl;
    this.netsCheckoutKey = environment.netsCheckoutKey;
    this.requestHeaders = [{ key: 'is-mobile-app', value: 'true' }];
  }
}
