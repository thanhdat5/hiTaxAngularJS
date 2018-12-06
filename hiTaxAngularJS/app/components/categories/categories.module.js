/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.categories', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('categories', {
				url: "/categories",
				parent: 'base',
				templateUrl: "/app/components/categories/categoryListView.html",
				controller: "categoryListController"
			});
	}
})();