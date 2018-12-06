/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.companies', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('companies', {
				url: "/companies",
				parent: 'base',
				templateUrl: "/app/components/companies/companyListView.html",
				controller: "companyListController"
			});
	}
})();