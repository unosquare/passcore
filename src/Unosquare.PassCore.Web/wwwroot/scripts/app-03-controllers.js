(function () {
    'use strict';

    angular.module('app')
        .controller('TitleCtrl', [
            '$scope', '$route', '$location', '$routeParams', 'ViewOptions',
            function ($scope, $route, $location, $routeParams, ViewOptions) {
                var me = this;
                me.content = ViewOptions.ApplicationTitle;

                $scope.$on('$routeChangeSuccess', function () {
                    me.content = $route.current.title + " - " + ViewOptions.ApplicationTitle;
                });
            }
        ]).controller('ChangePasswordCtrl', [
            '$scope', '$http', 'ViewOptions',
            function ($scope, $http, ViewOptions) {
                var me = this;

                $scope.ViewOptions = ViewOptions;

                $scope.Form = {
                    Username: '',
                    CurrentPassword: '',
                    NewPassword: '',
                    NewPasswordVerify: '',
                };

                $scope.Submit = function () {
                    $('div.form-overlay').show();
                    $http({
                        method: 'POST',
                        url: 'api/password',
                        data: $scope.Form,
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }).then(function successCallback(response) {
                        $('div.form-overlay').hide();
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        $('div.form-overlay').hide();
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }
            }
        ]);
})();