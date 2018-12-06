/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.products', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('products', {
				url: "/products",
				parent: 'base',
				templateUrl: "/app/components/products/productListView.html",
				controller: "productListController"
			});
	}
})();