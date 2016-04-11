$(function () {

    jQuery.validator.addMethod('validdate', function (value, element, params) {
        var currentDate = new Date();
        if (Date.parse(value) > currentDate) {
            return false;
        }
        return true;
    }, '');

    jQuery.validator.unobtrusive.adapters.add('validdate', function (options) {
        options.rules['validdate'] = {};
        options.messages['validdate'] = options.message;
    });

}(jQuery));