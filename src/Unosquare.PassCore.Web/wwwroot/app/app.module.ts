import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ChangePasswordComponent } from './app.change-password';
import { MaterialModule } from './app.material-module';

@NgModule({
  declarations: [
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MaterialModule,
  ],
  providers: [Options],
  bootstrap: [ChangePasswordComponent]
})
export class AppModule { }
