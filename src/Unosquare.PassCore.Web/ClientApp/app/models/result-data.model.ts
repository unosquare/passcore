import Error from './error.model'

export default class Result {
    public hasErrors: boolean;
    public errors: Error[];
    public payload: any;
}