import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
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
export class TransactionsListComponent implements AfterViewInit {
  displayedColumns: string[] = ['id', 'email', 'amount', 'currency', 'country'];
  dataSource = new MatTableDataSource<Transaction>(TRANSACTIONS_DATA);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }
}
