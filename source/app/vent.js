define(function(require){
  var _ = require('underscore');
  var Backbone = require('backbone');

  // Create an object for global events
  return _.extend({}, Backbone.Events);
});