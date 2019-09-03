
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

export interface IGlobalContext {
    alerts: IAlerts;
    applicationTitle: string;
    changePasswordForm: IChangePasswordForm;
    changePasswordTitle: string;
    forcePasswordGeneration: boolean;
    errorsPasswordForm: IErrorsPasswordForm;
    recaptcha: IRecaptcha;
    showPasswordMeter: boolean;
    useEmail: boolean;
    validationRegex: IValidationRegex;
}

export interface ISnackbarContext {
    sendMessage: (messageText: string, messageType?: string) => void;
}
