(function (app) {
    app.filter('germanyFormatNumberFilter', function () {
        return function (input) {
			try {
				return input.toString("#.###");
			} catch (e) {
				return input;
			}
        }
    });
})(angular.module('hiTax.common'));