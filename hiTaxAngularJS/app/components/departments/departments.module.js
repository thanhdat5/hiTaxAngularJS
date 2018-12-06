/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.departments', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('departments', {
				url: "/departments",
				parent: 'base',
				templateUrl: "/app/components/departments/departmentListView.html",
				controller: "departmentListController"
			});
	}
})();