import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http'

import { MaterialModule } from './app.material-module';

import { FlexLayoutModule } from "@angular/flex-layout";
import { ChangePasswordComponent } from './app.change-password';
import { Options } from './app.options';

@NgModule({
  declarations: [
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MaterialModule,
    HttpModule,
    FlexLayoutModule
  ],
  providers: [Options],
  bootstrap: [ChangePasswordComponent]
})
export class AppModule { }
