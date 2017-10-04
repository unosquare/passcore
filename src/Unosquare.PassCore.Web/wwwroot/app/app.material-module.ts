import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdIconModule,
  MdCardModule,
  MdInputModule,
  MatProgressBarModule,
  MatTooltipModule,
  MatToolbarModule
} from '@angular/material';

@NgModule({
  imports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatToolbarModule
  ],
  exports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatToolbarModule
  ]
})
export class MaterialModule {}