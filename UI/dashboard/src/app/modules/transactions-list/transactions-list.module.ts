import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TransactionsListRoutingModule } from './transactions-list-routing.module';
import { TransactionsListComponent } from './transactions-list.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';

@NgModule({
  declarations: [TransactionsListComponent],
  imports: [
    CommonModule,
    TransactionsListRoutingModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSortModule,
  ],
})
export class TransactionsListModule {}