import { Component, OnInit } from '@angular/core';
import 'bcc-pay/dist/bcc-pay';

@Component({
  selector: 'app-sample',
  templateUrl: './sample.component.html',
  styleUrls: ['./sample.component.scss'],
})
export class SampleComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
