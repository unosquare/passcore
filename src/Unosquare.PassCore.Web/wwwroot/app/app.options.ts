import { Injectable } from "@angular/core";
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';

@Injectable()
export class Options {

    constructor(private _httpService: Http) {}

    apiValues: string;

    GetData(){
        return this._httpService.get('api/password').subscribe(values => {
            this.apiValues = values.json() as string;
            console.log(this.apiValues);
        });
    }
}