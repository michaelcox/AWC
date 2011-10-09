/* jQuery DOMdocument.ready */
$(document).ready(function () {
    clearSearchFields();
});
function clearSearchFields() {
    var _field = $('.search input');
    var _defaultVal = _field.val();

    // On focus - empty field
    _field.focus(function () {
        if ($(this).val() == _defaultVal)
            $(this).val('');
    });

    // On blur - refill if blank
    _field.blur(function () {
        if ($(this).val() == '')
            $(this).val(_defaultVal);
    });
}