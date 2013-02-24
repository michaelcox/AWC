define(function(require) {
  var Backbone = require('backbone');
  var _ = require('underscore');
  var template = require('jade!layouts/main');
  var vent = require('vent');


  return Backbone.View.extend({

    initialize: function() {
      _.bindAll(this);

      // Bind the login/logout global events
      vent.on('login', this.loginUser);
      vent.on('logout', this.logoutUser);

      // Bind to modal global events
      vent.on('show:flash', this.showFlashMessage);
      vent.on('hide:flash', this.hideFlashMessage);
    },

    render: function() {
      this.$el.html(template());

      return this;
    },

    /* Pass any new views to this function, which closes the previous
     * view properly and renders the new view
    */
    renderView: function(view, options) {
      if (this.currentView) {
        // Run any cleanup functions the view may have
        if (this.currentView.onClose && _.isFunction(this.currentView.onClose)) {
          this.currentView.onClose();
        }
        this.currentView.remove();
      }

      // Render the new view
      this.currentView = view;
      var content = this.$el.find('.container');

      // Append the new current view
      content.html('');
      content.append(this.currentView.render().el);

      if (options && options.nav) {
        this.$('ul.menu-primary li').removeClass('active');
        this.$('ul.menu-primary li[data-bind="' + options.nav + '"]').addClass('active');
      }

    },

    showNewClient: function() {
      var ClientEditView = require('views/clients/clientEditView');
      var view = new ClientEditView();
      this.renderView(view, {nav: 'clientNew'});
    },

    showWaitlist: function() {
      var WaitListView = require('views/schedule/waitlistView');
      var view = new WaitListView();
      this.renderView(view, {nav: 'waitlist'});
    },

    showSchedule: function() {
      var ScheduleView = require('views/schedule/scheduleView');
      var view = new ScheduleView();
      this.renderView(view, {nav: 'schedule'});
    },

    showReports: function() {
      var ReportsView = require('views/reports/reportsMainView');
      var view = new ReportsView();
      this.renderView(view, {nav: 'reports'});
    }

  });

});