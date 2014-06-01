"use strict"
mongoose = require("mongoose")
Launch = mongoose.model("Launch")

module.exports = (app, io) ->
  io.sockets.on "connection", (socket) ->
    socket.on 'subscribe', (data)->
      null
  application_launched = (attrs)->
    io.sockets.emit('application_launched', attrs)
  application_closed = (attrs)->
    io.sockets.emit('application_closed', attrs)
  return {
    application_closed
    application_launched
  }
