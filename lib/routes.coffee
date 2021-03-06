"use strict"
index = require("./controllers")
users = require("./controllers/users")
session = require("./controllers/session")
launches = require("./controllers/launches")
api = require("./controllers/api")
middleware = require("./middleware")
auth = require("./auth")

###
Application routes
###
module.exports = (app) ->

  # Server API Routes
#  app.get "/api/awesomeThings", api.awesomeThings

  app.post "/api/application_launched",
    api.application_launched(app.socket_functions.application_launched)
  app.post "/api/application_closed",
    api.application_closed(app.socket_functions.application_closed)
  # app.post "/api/users", users.create
  # app.put "/api/users", users.updateProfile
  # app.get "/api/users/me", users.me
  # app.get "/api/users/:id", users.show

  # app.post "/api/session", session.login
  # app.del "/api/session", session.logout
  # app.get "/auth/vkontakte", session.vkAuth
  # app.get "/auth/facebook", session.fbAuth
  # app.get "/auth/oauth/callback", session.oauthCallback, session.closeWindow


#  app.post "/api/conferences/:id/slides",
  #auth(admin:true, user: auth.checkOwner), conferences.uploadSlides
#  app.get "/api/conferences", conferences.index
#  app.get "/api/conferences/:id", conferences.show
#  app.post "/api/conferences", conferences.create
#  app.del "/api/conferences/:id", conferences.delete
#  app.put "/api/conferences/:id", conferences.update

  app.get "/launches", launches.index
  # All other routes to use Angular routing in app/scripts/app.js
  app.get "/partials/*", index.partials
  app.get "/*", index.index