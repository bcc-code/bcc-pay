import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SampleRoutingModule } from './sample-routing.module';
import { SampleComponent } from './sample.component';

@NgModule({
  declarations: [SampleComponent],
  imports: [CommonModule, SampleRoutingModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SampleModule {}
