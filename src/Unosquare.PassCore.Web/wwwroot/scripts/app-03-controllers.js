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
            '$scope', '$http', '$sce', 'vcRecaptchaService', 'ViewOptions',
            function ($scope, $http, $sce, recaptcha, ViewOptions) {
                var me = this;

                $scope.ViewOptions = ViewOptions;

                $scope.FormData = {
                    Username: '',
                    CurrentPassword: '',
                    NewPassword: '',
                    NewPasswordVerify: '',
                    Recaptcha: '',
                };

                $scope.EmptyFormData = angular.copy($scope.FormData);

                $scope.SetRecaptchaResponse = function (response) {
                    $scope.FormData.Recaptcha = response;
                };

                $scope.ClearRecaptchaResponse = function () {
                    $scope.FormData.Recaptcha = '';
                };

                $scope.Submit = function () {
                    $('div.form-overlay').show();
                    
                    $.each($scope.Form, function (index, field) {
                        if (!field) return;
                        if (field.$valid == undefined) return;

                        field.Validation = { HasError: false, ErrorMessage: '' };
                    });

                    $scope.Form.$setUntouched();
                    grecaptcha.reset();

                    $http({
                        method: 'POST',
                        url: 'api/password',
                        data: $scope.FormData,
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }).then(function successCallback(response) {
                        $('div.form-overlay').hide();
                        // this callback will be called asynchronously
                        // when the response is available
                        $scope.FormData = angular.copy($scope.EmptyFormData);
                    }, function errorCallback(response) {
                        $('div.form-overlay').hide();

                        $.each(response.data.Errors, function (index, errorData) {
                            if (errorData.ErrorType == 2) {
                                $scope.Form[errorData.FieldName].Validation.HasError = true;
                                $scope.Form[errorData.FieldName].Validation.ErrorMessage = $scope.ViewOptions.ErrorMessages[errorData.ErrorCode];
                            }
                        });
                    });
                }
            }
        ]);
})();