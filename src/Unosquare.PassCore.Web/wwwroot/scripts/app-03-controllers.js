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
            '$scope', 'ViewOptions',
            function ($scope, ViewOptions) {
                var me = this;

                $scope.ViewOptions = ViewOptions;

            }
        ]);
})();