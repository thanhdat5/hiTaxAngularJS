/// <reference path="../../../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax.profile', ['hiTax.common']).config(config);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('profile', {
				url: "/profile",
				parent: 'base',
				templateUrl: "/app/components/profile/profileView.html",
				controller: "profileController"
			});
	}
})();