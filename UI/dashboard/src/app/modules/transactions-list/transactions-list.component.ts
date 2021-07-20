import { Component, OnInit } from '@angular/core';
export interface Transaction {
  id: string;
  email: string;
  amount: number;
  currency: string;
  country: string;
}

const TRANSACTIONS_DATA: Transaction[] = [
  {
    id: '6aaa4d10',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '5be837dbc8ad',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '6aaa4d10',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '5be837dbc8ad',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '6aaa4d10',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '5be837dbc8ad',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '6aaa4d10',
    email: 'test@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
];

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss'],
})
export class TransactionsListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'email', 'amount', 'currency', 'country'];
  dataSource = TRANSACTIONS_DATA;

  constructor() {}

  ngOnInit(): void {
    console.log('Init transaction list');
  }
}
