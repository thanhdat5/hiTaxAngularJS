/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.customers', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('customers', {
				url: "/customers",
				parent: 'base',
				templateUrl: "/app/components/customers/customerListView.html",
				controller: "customerListController"
			});
	}
})();