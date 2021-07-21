import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TransactionsListRoutingModule } from './transactions-list-routing.module';
import { TransactionsListComponent } from './transactions-list.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
  declarations: [TransactionsListComponent],
  imports: [
    CommonModule,
    TransactionsListRoutingModule,
    MatTableModule,
    MatPaginatorModule,
  ],
})
export class TransactionsListModule {}
