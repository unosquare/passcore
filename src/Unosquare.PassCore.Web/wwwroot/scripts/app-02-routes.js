(function() {
    'use strict';

    angular.module('app').config([
        '$routeProvider', '$locationProvider', 'ViewOptions', function ($routeProvider, $locationProvider, ViewOptions) {
            $routeProvider.
                when('/', {
                    templateUrl: 'views/change-password.html',
                    title: ViewOptions.ChangePasswordTitle
                }).otherwise({
                    redirectTo: '/'
                });

            $locationProvider.html5Mode(true);
        }
    ]);
})();