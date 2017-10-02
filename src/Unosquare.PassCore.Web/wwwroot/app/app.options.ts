import { Injectable } from "@angular/core";
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';

@Injectable()
export class Options {
    constructor(private http: Http) {}

    GetData(){
        return this.http.get('api/password').map(this.ExtractData);
    }

    private ExtractData(res: Response){
        let body = res.text();
        return body;
    }
}