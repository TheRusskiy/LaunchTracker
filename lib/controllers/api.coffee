"use strict"
mongoose = require("mongoose")
Launch = mongoose.model("Launch")


exports.application_launched = (callback)->
  (req, res, next) ->
    machine = req.body.machine
    application = req.body.application
    launch = new Launch({
      machine: machine
      application: application
      started_at: Date.now
      })
    console.log req.body
    launch.save (err) ->
      if err
        if err then console.log err
      callback(launch.toObject())

exports.application_closed = (callback)->
  (req, res, next) ->
    machine = req.body.machine
    application = req.body.application
    newUser = new Launch({
      machine: machine
      application: application
      started_at: Date.now()
      })
    console.log req.body

    Launch
      .find({ machine: machine, closed_at: null })
      .sort('-started_at')
      .exec((err, launches)->
        last = launches[0]
        last.closed_at = Date.now()
        last.save (err)->
          console.log err
          callback(last.toObject())
      );