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
  MatProgressSpinnerModule,
  MdDialogModule
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
    MatProgressSpinnerModule,
    MdDialogModule
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
    MatProgressSpinnerModule,
    MdDialogModule
  ]
})
export class MaterialModule {}