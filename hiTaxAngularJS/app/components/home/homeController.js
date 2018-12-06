(function (app) {
	app.controller("homeController", homeController);
	homeController.$inject = ['$rootScope'];
	function homeController($rootScope) {
		$rootScope.pageTitle = "Dashboard";
	}
})(angular.module('hiTax'));