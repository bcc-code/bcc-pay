import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: '/list', pathMatch: 'full' },
  {
    path: 'list',
    loadChildren: () =>
      import('./modules/transactions-list/transactions-list.module').then(
        (m) => m.TransactionsListModule
      ),
  },
  {
    path: 'sample',
    loadChildren: () =>
      import('./modules/sample/sample.module').then((m) => m.SampleModule),
  },
  {
    path: 'status',
    loadChildren: () =>
      import('./modules/payment-status/payment-status.module').then(
        (m) => m.PaymentStatusModule
      ),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
