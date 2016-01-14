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
            NewPasswordHelpblock: 'Enter a strong password. You can use <a href="http://passwordsgenerator.net/" target="_blank">this tool</a> to help you create one.',
            NewPasswordVerifyLabel: "Re-enter New Password",
            NewPasswordVerifyPlaceholder: "",
            NewPasswordVerifyHelpblock: "Enter your new password again"
        },
        ErrorMessages: {
            0: "General Error",
            1: "Field is required",
            2: "Fields don't match",
            3: "User was not found",
            4: "The credentials provided are invalid"
        },
        Recaptcha: {
            IsEnabled: true,
            SiteKey: "000000000000000000000-000000000000000000"
        }
    });
})();