import * as React from "react";

interface IAlerts {
    errorCaptcha: string;
    errorComplexPassword: string;
    errorConnectionLdap: string;
    errorFieldMismatch: string;
    errorFieldRequired: string;
    errorInvalidCredentials: string;
    errorInvalidDomain: string;
    errorInvalidUser: string;
    errorPasswordChangeNotAllowed: string;
    successAlertBody: string;
    successAlertTitle: string;
}

interface IChangePasswordForm {
    changePasswordButtonLabel: string;
    currentPasswordHelpblock: string;
    currentPasswordLabel: string;
    helpText: string;
    newPasswordHelpblock: string;
    newPasswordLabel: string;
    newPasswordVerifyHelpblock: string;
    newPasswordVerifyLabel: string;
    usernameDefaultDomainHelperBlock: string;
    usernameHelpblock: string;
    usernameLabel: string;
}

interface IErrorsPasswordForm {
    fieldRequired: string;
    passwordMatch: string;
    usernameEmailPattern: string;
    usernamePattern: string;
}

interface IRecaptcha {
    languageCode: string;
    siteKey: string;
    privateKey: string;
}

interface IValidationRegex {
    emailRegex: string;
    usernameRegex: string;
}

interface IGlobalContext {
    alerts: IAlerts;
    applicationTitle: string;
    changePasswordForm: IChangePasswordForm;
    changePasswordTitle: string;
    errorsPasswordForm: IErrorsPasswordForm;
    recaptcha: IRecaptcha;
    showPasswordMeter: boolean;
    useEmail: boolean;
    validationRegex: IValidationRegex;
}

export const GlobalContext = React.createContext<IGlobalContext>({
    alerts: null,
    applicationTitle: '',
    changePasswordForm: null,
    changePasswordTitle: '',
    errorsPasswordForm: null,
    recaptcha: null,
    showPasswordMeter: false,
    useEmail: false,
    validationRegex: null,
});

interface ISnackbarContext {
    sendMessage: (messageText: string, messageType?: string) => void;
}

export const SnackbarContext = React.createContext<ISnackbarContext>({
    sendMessage: null,
});

interface IGlobalActionsContext {
    changePassword: any;
}

export const GlobalActionsContext = React.createContext<IGlobalActionsContext>({
    changePassword: null,
});
