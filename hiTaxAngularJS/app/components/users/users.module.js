/// <reference path="../../../content/template/libs/angular/angular.js" />

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
			}).state('user_add', {
				url: "/user_add",
				parent: 'base',
				templateUrl: "/app/components/users/userAddView.html",
				controller: "userAddController"
			}).state('user_edit', {
				url: "/user_edit/:id",
				parent: 'base',
				templateUrl: "/app/components/users/userEditView.html",
				controller: "userEditController"
			});
	}
})();