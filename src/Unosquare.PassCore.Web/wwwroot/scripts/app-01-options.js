(function () {
    'use strict';

    var app = angular.module("app");

    fetchData().then(bootstrapApplication);

    function fetchData() {
        var initInjector = angular.injector(["ng"]);
        var $http = initInjector.get("$http");

        return $http.get("api/password").then(function (response) {
            app.constant("ViewOptions", response.data);

            // Insert recaptcha if necessary
            if (response.data.Recaptcha.IsEnabled === true) {
                var sp = document.createElement('script'); sp.type = 'text/javascript'; sp.async = true; sp.defer = true;
                sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + response.data.Recaptcha.LanguageCode;
                var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(sp, s);
            }
        }, function (errorResponse) {
            // Handle error case
        });
    }

    function bootstrapApplication() {
        angular.element(document).ready(function () {
            angular.bootstrap(document, ["app"]);
        });
    }
})();