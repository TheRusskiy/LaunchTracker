"use strict"

# Intercept 401s and 403s and redirect you to login
angular.module("radioApp", ["ngCookies", "ngResource", "ngSanitize", "ngRoute", "ngAnimate"]).config(($routeProvider, $locationProvider, $httpProvider) ->
  $routeProvider.when("/",
    templateUrl: "partials/main"
    controller: "MainCtrl"
  ).otherwise redirectTo: "/"
  $locationProvider.html5Mode true
  $httpProvider.interceptors.push ["$q", "$location", ($q, $location) ->
    responseError: (response) ->
      if response.status is 401 or response.status is 403
        $location.path "/login"
        $q.reject response
      else
        $q.reject response
  ]
).run ($rootScope, $location) ->

  # Redirect to login if route requires auth and you're not logged in
  # $rootScope.$on "$routeChangeStart", (event, next) ->
    # $location.path "/login"  if next.authenticate and not Auth.isLoggedIn()

