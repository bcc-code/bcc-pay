import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-list-transactions-page',
  templateUrl: './list-transactions-page.component.html',
  styleUrls: ['./list-transactions-page.component.scss'],
})
export class ListTransactionsPageComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {
    console.log('List transactions page init');
  }
}
