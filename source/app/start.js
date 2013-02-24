/*globals requirejs */

requirejs.config({
  urlArgs: "bust=" + (new Date()).getTime(),
  paths: {
    'jquery'              : '../libs/jquery-1.9.1',
    'underscore'          : '../libs/underscore-1.4.3',
    'backbone'            : '../libs/backbone-0.9.10',
    'moment'              : '../libs/moment-1.7.2',
    'jade'                : '../libs/jade'
  },
  shim: {
    'underscore': {
      exports: '_'
    },
    'backbone': {
      deps: ['underscore', 'jquery'],
      exports: 'Backbone'
    }
  }
});

define(function(require) {
  var Backbone = require('backbone');
  var Router = require('router');
  var globals = require('globals');
  var $ = require('jquery');
  var AppView = require('appView');

  // Create the router
  globals.router = new Router();

  // Create the main view
  globals.appView = new AppView();
  $('body').html(globals.appView.render().el);

  // Start the Router
  Backbone.history.start();

});