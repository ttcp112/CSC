var app = angular.module('app', ['ngRoute', 'ngCookies']);
app.config(['$provide', '$routeProvider', '$httpProvider', '$compileProvider', function ($provide, $routeProvider, $httpProvider, $compileProvider) {

    //================================================
    // Routes
    //================================================
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|file|javascript):/);
}]);


