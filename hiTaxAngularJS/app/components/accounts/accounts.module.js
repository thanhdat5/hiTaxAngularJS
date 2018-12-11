/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.accounts', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('accounts', {
				url: "/accounts",
				parent: 'base',
				templateUrl: "/app/components/accounts/accountListView.html",
				controller: "accountListController"
			});
	}
})();