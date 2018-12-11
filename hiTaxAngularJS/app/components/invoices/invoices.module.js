/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.invoices', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('invoices', {
				url: "/invoices",
				parent: 'base',
				templateUrl: "/app/components/invoices/invoiceListView.html",
				controller: "invoiceListController"
			});
	}
})();