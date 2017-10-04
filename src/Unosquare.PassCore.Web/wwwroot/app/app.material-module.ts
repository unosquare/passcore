import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdIconModule,
  MdCardModule,
  MdInputModule,
  MatProgressBarModule,
  MatTooltipModule,
  MatToolbarModule,
  MdSnackBarModule,
  MatProgressSpinnerModule
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
    MdSnackBarModule,
    MatProgressSpinnerModule
  ],
  exports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatToolbarModule,
    MdSnackBarModule,
    MatProgressSpinnerModule
  ]
})
export class MaterialModule {}