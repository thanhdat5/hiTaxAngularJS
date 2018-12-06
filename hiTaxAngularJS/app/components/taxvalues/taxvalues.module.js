/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.taxvalues', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('taxvalues', {
				url: "/taxvalues",
				parent: 'base',
				templateUrl: "/app/components/taxvalues/taxvalueListView.html",
				controller: "taxvalueListController"
			});
	}
})();