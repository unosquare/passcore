import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { ChangePasswordComponent } from './app.change-password';

@NgModule({
  declarations: [
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [ChangePasswordComponent]
})
export class AppModule { }
