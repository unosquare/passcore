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
            NewPasswordAgainLabel: "Re-enter New Password",
            NewPasswordAgainPlaceholder: "",
            NewPasswordAgainHelpblock: "Enter your new password again"
        }
    });
})();