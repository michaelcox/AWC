define(function(require) {
  var Backbone = require('backbone');
  var _ = require('underscore');
  var template = require('jade!./templates/clientEdit');
  var globals = require('globals');
  var vent = require('vent');

  return Backbone.View.extend({

    /**
     * @param options.model Client model
    */
    initialize: function() {
      _.bindAll(this);
    },

    render: function() {
      this.$el.html(template({}));
      return this;
    }
  });

});
