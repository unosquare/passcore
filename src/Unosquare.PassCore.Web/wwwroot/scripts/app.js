(function () {
    'use strict';

    var app = angular.module('app', ['ngRoute', 'ui.bootstrap']);
    app.controller('TitleCtrl', [
        '$scope', '$route', '$location', '$routeParams',
        function ($scope, $route, $location, $routeParams) {
            var me = this;
            me.content = "Sample";
            me.pageTitle = "Loading . . .";
            me.key = "Loading . . .";

            $scope.$on('$routeChangeSuccess', function () {
                $scope.subheader = null;

                me.key = $route.current.title;
                me.pageTitle = me.key;
                if ($routeParams.param) me.pageTitle += " - " + $routeParams.param;
                me.content = me.pageTitle + " - Sample";
            });
        }
    ]);

    app.config([
        '$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider.
                when('/', {
                    templateUrl: 'views/home.html',
                    title: 'Home',
                    print: true
                }).when('/Login', {
                    templateUrl: 'views/login.html',
                    title: 'Login'
                }).otherwise({
                    redirectTo: '/'
                });

            $locationProvider.html5Mode(true);
        }
    ])
})();