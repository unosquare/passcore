import { Options } from './app.options'
import { Component } from '@angular/core';
import { Constants } from './Constants';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html',
  providers:[Options]
})
export class ChangePasswordComponent {
  constructor(private http:Http, private pt: Options){}
  ViewOptions = this.pt.GetData();
  title = 'Change Password';  
}
