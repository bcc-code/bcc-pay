import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TransactionsListRoutingModule } from './transactions-list-routing.module';
import { TransactionsListComponent } from './transactions-list.component';
import { MatTableModule } from '@angular/material/table';

@NgModule({
  declarations: [TransactionsListComponent],
  imports: [CommonModule, TransactionsListRoutingModule, MatTableModule],
})
export class TransactionsListModule {}
