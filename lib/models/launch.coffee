"use strict"
mongoose = require("mongoose")
Schema = mongoose.Schema

###
Conference Schema
###
LaunchSchema = new Schema(
#  _author : { type: Schema.ObjectId, ref: 'User' }
  machine: String
  application: String
  started_at: { type: Date}
  finished_at: { type: Date}
)

mongoose.model('Launch', LaunchSchema);
