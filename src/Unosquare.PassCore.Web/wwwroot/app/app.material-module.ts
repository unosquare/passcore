import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdIconModule,
  MdCardModule,
  MdInputModule,
  MatProgressBarModule
} from '@angular/material';

@NgModule({
  imports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule
  ],
  exports: [
    MdButtonModule,
    MdIconModule,
    MdCardModule,
    MdInputModule,
    MatProgressBarModule
  ]
})
export class MaterialModule {}