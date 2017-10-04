import { Error } from './error.model'

export class Result {
    constructor(hasErrors: boolean, errors: Error[], payload: any) {
        this.errors = errors;
        this.hasErrors = hasErrors;
        this.payload = payload;
    }
    public hasErrors: boolean;
    public errors: Error[];
    public payload: any;
}