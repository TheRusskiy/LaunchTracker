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
      started_at: new Date
      })
    console.log req.body
    launch.save (err) ->
      if err
        console.log err
        res.json 500, err
      callback(launch.toObject())
      res.send(204)

exports.application_closed = (callback)->
  (req, res, next) ->
    machine = req.body.machine
    application = req.body.application
    newUser = new Launch({
      machine: machine
      application: application
      started_at: new Date
      })
    console.log req.body

    Launch
      .find({ machine: machine, closed_at: null })
      .sort('-started_at')
      .exec((err, launches)->
        last = launches[0]
        last.closed_at = Date.now()
        last.save (err)->
          if err
            console.log err
            res.json 500, err
          res.send(204)
          callback(last.toObject())
      );
