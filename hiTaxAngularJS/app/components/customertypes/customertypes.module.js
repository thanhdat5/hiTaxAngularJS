/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.customertypes', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('customertypes', {
				url: "/customertypes",
				parent: 'base',
				templateUrl: "/app/components/customertypes/customertypeListView.html",
				controller: "customertypeListController"
			});
	}
})();