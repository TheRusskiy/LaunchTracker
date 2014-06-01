"use strict"
angular.module("radioApp").factory "Launch", ($resource) ->
  Launch = $resource "/launches",
    query:
      method: "GET"
      isArray:true
      params:
        action: "index"

  return Launch