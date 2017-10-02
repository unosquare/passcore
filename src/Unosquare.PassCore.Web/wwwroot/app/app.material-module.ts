import { NgModule } from '@angular/core';

import {
  MdButtonModule,
  MdMenuModule,
  MdToolbarModule,
  MdIconModule,
  MdCardModule
} from '@angular/material';

@NgModule({
  imports: [
    MdButtonModule
  ],
  exports: [
    MdButtonModule
  ]
})
export class MaterialModule {}