﻿/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.users', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('users', {
				url: "/users",
				parent: 'base',
				templateUrl: "/app/components/users/userListView.html",
				controller: "userListController"
			});
	}
})();