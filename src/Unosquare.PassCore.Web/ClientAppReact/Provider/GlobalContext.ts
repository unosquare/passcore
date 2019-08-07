import * as React from "react";

interface IAlerts {
    ErrorCaptcha: string;
    ErrorComplexPassword: string;
    ErrorConnectionLdap: string;
    ErrorFieldMismatch: string;
    ErrorFieldRequired: string;
    ErrorInvalidCredentials: string;
    ErrorInvalidDomain: string;
    ErrorInvalidUser: string;
    ErrorPasswordChangeNotAllowed: string;
    SuccessAlertBody: string;
    SuccessAlertTitle: string;
}

interface IChangePasswordForm {
    ChangePasswordButtonLabel: string;
    CurrentPasswordHelpblock: string;
    CurrentPasswordLabel: string;
    HelpText: string;
    NewPasswordHelpblock: string;
    NewPasswordLabel: string;
    NewPasswordVerifyHelpblock: string;
    NewPasswordVerifyLabel: string;
    UsernameDefaultDomainHelperBlock: string;
    UsernameHelpblock: string;
    UsernameLabel: string;
}

interface IErrorsPasswordForm {
    FieldRequired: string;
    PasswordMatch: string;
    UsernameEmailPattern: string;
    UsernamePattern: string;
}

interface IRecaptcha {
    LanguageCode: string;
    SiteKey: string;
    PrivateKey: string;
}

interface IValidationRegex {
    EmailRegex: string;
    UsernameRegex: string;
}

interface IGlobalContext {
    Alerts: IAlerts;
    ApplicationTitle: string;
    ChangePasswordForm: IChangePasswordForm;
    ChangePasswordTitle: string;
    ErrorsPasswordForm: IErrorsPasswordForm;
    Recaptcha: IRecaptcha;
    ShowPasswordMeter: boolean;
    UseEmail: boolean;
    ValidationRegex: IValidationRegex;
}

export const GlobalContext = React.createContext<IGlobalContext>({
    Alerts: null,
    ApplicationTitle: '',
    ChangePasswordForm: null,
    ChangePasswordTitle: '',
    ErrorsPasswordForm: null,
    Recaptcha: null,
    ShowPasswordMeter: false,
    UseEmail: false,
    ValidationRegex: null,
});

export const GlobalActionsContext = React.createContext<any>({});
