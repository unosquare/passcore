import { error } from './error.model'

export class Errors{
    public hasErrors:boolean;
    public errors:error[];
    public payload:any;
}