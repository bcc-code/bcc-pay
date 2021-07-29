import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
export interface Transaction {
  id: string;
  email: string;
  amount: number;
  currency: string;
  country: string;
}

export interface Payment {
  paymentId: string;
  paymentIdForCheckoutForm: string;
  payerId: string;
  currencyCode: string;
  amount: number;
  countryCode: string;
  created: string;
  updated: string;
  paymentStatus: string;
  paymentMethod: string;
}

const TRANSACTIONS_DATA: Transaction[] = [
  {
    id: '0aaa4d10',
    email: 'test0@test.no',
    amount: 50,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '52e837dbc8ad',
    email: 'test@test.no',
    amount: 40,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '6baa4d10',
    email: 't@test.no',
    amount: 55,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '5be837dbc8ad',
    email: 'test1@test.no',
    amount: 1,
    currency: 'BTC',
    country: 'ES',
  },
  {
    id: '3aaa4d10',
    email: 'test@test.no',
    amount: 30,
    currency: 'NOK',
    country: 'NO',
  },
  {
    id: '5ce837dbc8ad',
    email: 'test2@test.no',
    amount: 10,
    currency: 'USD',
    country: 'US',
  },
  {
    id: '6faa4d10',
    email: 'test@test.no',
    amount: 60,
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

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public applyFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  };
}
