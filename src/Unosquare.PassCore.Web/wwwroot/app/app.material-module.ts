import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdIconModule,
  MdCardModule,
  MdInputModule,
  MatProgressBarModule,
  MatTooltipModule,
  MatToolbarModule,
  MdSnackBarModule
} from '@angular/material';

@NgModule({
  imports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatToolbarModule,
    MdSnackBarModule
  ],
  exports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatToolbarModule,
    MdSnackBarModule
  ]
})
export class MaterialModule {}