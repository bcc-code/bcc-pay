import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PaymentStatusRoutingModule } from './payment-status-routing.module';
import { PaymentStatusComponent } from './payment-status.component';

@NgModule({
  declarations: [PaymentStatusComponent],
  imports: [CommonModule, PaymentStatusRoutingModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class PaymentStatusModule {}
