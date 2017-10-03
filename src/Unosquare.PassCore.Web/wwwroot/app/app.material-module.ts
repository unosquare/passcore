import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdIconModule,
  MdCardModule,
  MdInputModule,
  MatProgressBarModule,
  MatTooltipModule
} from '@angular/material';

@NgModule({
  imports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  exports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule
  ]
})
export class MaterialModule {}