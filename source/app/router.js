define(function(require){
  var Backbone = require('backbone');
  var globals = require('globals');
  var vent = require('vent');

  return Backbone.Router.extend({
    routes: {
      ''                               : 'root',
      'clients/new'                    : 'clientNew',
      'waitlist'                       : 'waitlist',
      'schedule'                       : 'schedule',
      'reports'                        : 'reports'
    },

    root: function() {
      this.navigate('clients/new', {trigger: true});
    },

    // CLIENTS
    clientNew: function() {
      globals.appView.showNewClient();
    },

    // WAIT LIST
    waitlist: function() {
      globals.appView.showWaitlist();
    },

    // SCHEDULE
    schedule: function() {
      globals.appView.showSchedule();
    },

    // REPORTS
    reports: function() {
      globals.appView.showReports();
    }

  });


});