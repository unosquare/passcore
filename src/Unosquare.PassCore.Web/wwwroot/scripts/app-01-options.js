(function () {
    'use strict';

    angular.module('app').constant("ViewOptions", {
        ApplicationTitle: "Self-Service Account Management Tools",
        ChangePasswordTitle: "Change Account Password",
        ChangePasswordForm: {
            UsernameLabel: "Username",
            UsernamePlaceholder: "username@domain.com",
            UsernameHelpblock: "Your organization's username",
            CurrentPasswordLabel: "Current Password",
            CurrentPasswordPlaceholder: "",
            CurrentPasswordHelpblock: "Enter your current password",
            NewPasswordLabel: "New Password",
            NewPasswordPlaceholder: "",
            NewPasswordHelpblock: "Enter a strong password. You can use <a href='http://passwordsgenerator.net/' target='_blank'>this tool</a> to help you create one.",
            NewPasswordVerifyLabel: "Re-enter New Password",
            NewPasswordVerifyPlaceholder: "",
            NewPasswordVerifyHelpblock: "Enter your new password again"
        },
        ErrorMessages: {
            0: "System Error",
            1: "Field is required",
            2: "Fields don't match",
            3: "We could not find you user account",
            4: "The username and current password you provided are invalid",
            5: "Could not verify you are not a robot"
        },
        Recaptcha: {
            IsEnabled: false,
            SiteKey: "000000000000000000000-000000000000000000"
        },
        Alerts: {
            SuccessAlertTitle: "You have changed your password successfully.",
            SuccessAlertBody: "Please note it may take a few hours for your new password to reach all domain controllers.",
            ErrorAlertTitle: "There was an error changing your password",
            ErrorAlertBody: "Error Information: "
        }
    });
})();