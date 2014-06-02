"use strict"
angular.module("radioApp").controller "MainCtrl", 
  ($scope, $http, $rootScope, Socket, Launch) ->
    $scope.launches = Launch.query()
    Socket($scope).on 'application_launched', (obj)->
      console.log obj
      $scope.launches.push obj
    Socket($scope).on 'application_closed', (obj)->
      console.log obj
      for launch, index in $scope.launches
        if launch._id == obj._id
          $scope.launches.splice index, 1
          break
      $scope.launches.push obj
        
      
    $scope.format_date = (date)->
      return "-" unless date
      date = new Date(date)
      date.toLocaleString()
    $scope.time_difference = (from, to)->
      if from and to
        (new Date(to)-new Date(from))/1000 + " sec"
      else 
        "-"

