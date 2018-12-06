(function (app) {
	app.controller("configsController", configsController);
	configsController.$inject = ['$rootScope'];
	function configsController($rootScope) {
		$rootScope.pageTitle = "Site Configs";
	}
})(angular.module('hiTax'));