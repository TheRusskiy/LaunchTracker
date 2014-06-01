"use strict"
module.exports =
  env: "production"
  host: "launch_tracker.herokuapp.com"
  socketio:
    log_level: 1

  mongo:
    uri: "mongodb://therusskiy:w0rdpazz@kahana.mongohq.com:10072/launch_tracker" or "mongodb://localhost/launch_tracker_production"