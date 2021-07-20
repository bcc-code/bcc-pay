import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListTransactionsPageComponent } from './list-transactions-page/list-transactions-page.component';

const routes: Routes = [{ path: '', component: ListTransactionsPageComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ListTransactionsRoutingModule {}
