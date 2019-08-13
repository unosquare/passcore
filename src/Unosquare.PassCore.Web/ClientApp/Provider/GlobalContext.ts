import * as React from "react";
import { IGlobalContext, ISnackbarContext } from "../types/Providers";

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

export const SnackbarContext = React.createContext<ISnackbarContext>({
    sendMessage: null,
});
