import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { Options } from './app.options';
import { ChangePasswordComponent } from './app.change-password';
import { MaterialModule } from './app.material-module'

@NgModule({
  declarations: [
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    MaterialModule,
    HttpModule,
    FormsModule
  ],
  providers: [Options],
  bootstrap: [ChangePasswordComponent]
})
export class AppModule { }
