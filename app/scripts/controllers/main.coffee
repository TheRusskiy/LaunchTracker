"use strict"
angular.module("radioApp").controller "MainCtrl", 
  ($scope, $http, $rootScope, Socket, Launch) ->
  	$scope.launches = Launch.query()
    # Socket($scope).on 'guest_nickname', (nickname)->
      # $rootScope.guestNickname = nickname
      # $cookieStore.put('preferredNickname', nickname);

