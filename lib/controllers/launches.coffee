"use strict"
mongoose = require("mongoose")
Launch = mongoose.model("Launch")


exports.index = (req, res, next) ->
  pojos = []
  Launch
    .find({})
    .sort('-started_at')
    .exec((err, launches)->
      for launch in launches
        pojos.push launch.toObject()
      res.send pojos
    );