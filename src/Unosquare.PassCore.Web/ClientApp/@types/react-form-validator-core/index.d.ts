declare module 'react-form-validator-core' {
    export class ValidatorComponent extends React.Component<any> {}

    export class ValidatorForm extends React.Component<any> {
        public static addValidationRule(ruleName: string, callback: any): void;
    }
}
