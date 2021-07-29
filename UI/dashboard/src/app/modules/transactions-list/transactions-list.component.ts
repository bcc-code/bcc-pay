import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

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

const TRANSACTIONS_DATA: Payment[] = [
  {
    paymentId: '123',
    paymentIdForCheckoutForm: '12345',
    payerId: '123',
    currencyCode: 'NOK',
    amount: 12,
    countryCode: 'NOR',
    created: 'date',
    updated: 'date',
    paymentStatus: 'paid',
    paymentMethod: 'nets',
  },
  {
    paymentId: '321',
    paymentIdForCheckoutForm: '123456',
    payerId: '321',
    currencyCode: 'NOK',
    amount: 12,
    countryCode: 'NOR',
    created: 'date',
    updated: 'date',
    paymentStatus: 'paid',
    paymentMethod: 'nets',
  },
];

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss'],
})
export class TransactionsListComponent implements AfterViewInit {
  displayedColumns: string[] = [
    'paymentId',
    'paymentIdForCheckoutForm',
    'payerId',
    'currencyCode',
    'amount',
    'countryCode',
    'created',
    'updated',
    'paymentStatus',
    'paymentMethod',
  ];
  dataSource = new MatTableDataSource<Payment>(TRANSACTIONS_DATA);

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
