(function () {
    'use strict';
    
    angular.module('app', ['ngRoute',
        'ui.bootstrap'
    ]).config([
        '$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider.
                when('/', {
                    templateUrl: '/ui/views/home.html',
                    title: 'Home',
                    print: true
                }).when('/Login', {
                    templateUrl: '/ui/views/login.html',
                    title: 'Login'
                }).otherwise({
                    redirectTo: '/'
                });

            $locationProvider.html5Mode(true);
        }
    ]);
})();