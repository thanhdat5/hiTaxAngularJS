/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.units', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('units', {
				url: "/units",
				parent: 'base',
				templateUrl: "/app/components/units/unitListView.html",
				controller: "unitListController"
			});
	}
})();