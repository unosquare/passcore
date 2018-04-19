import { Error } from './error.model'

export class Result {
    public hasErrors: boolean;
    public errors: Error[];
    public payload: any;
}