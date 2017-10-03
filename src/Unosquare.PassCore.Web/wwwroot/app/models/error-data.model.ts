import { error } from './error.model'

export class Result{
    constructor(hasErrors:boolean, errors:error[], payload:any)
    {
        this.errors = errors;
        this.hasErrors = hasErrors;
        this.payload = payload;
    }
    public hasErrors:boolean;
    public errors:error[];
    public payload:any;
}