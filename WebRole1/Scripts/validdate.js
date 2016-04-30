$(function () {

    jQuery.validator.addMethod('validdate', function (value, element, params) {
        if (!/Invalid|NaN/.test(new Date(value))) {
            return new Date(value) > new Date();
        }
        return isNaN(value) && isNaN($(params).val()) || (parseFloat(value) > parseFloat($(params).val()));
    }, '');

    jQuery.validator.unobtrusive.adapters.add('validdate', {}, function (options) {
        options.rules['validdate'] = true;
        options.messages['validdate'] = options.message;
    });

    //jQuery.validator.addMethod('validdate', function (value, element, params) {
    //    var currentDate = new Date();
    //    if (Date.parse(value) > currentDate) {
    //        return false;
    //    }
    //    return true;
    //}, '');

    //jQuery.validator.unobtrusive.adapters.add('validdate', function (options) {
    //    options.rules['validdate'] = {};
    //    options.messages['validdate'] = options.message;
    //});

}(jQuery));