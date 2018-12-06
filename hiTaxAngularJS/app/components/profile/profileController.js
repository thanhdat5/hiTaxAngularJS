(function (app) {
	app.controller("profileController", profileController);
	profileController.$inject = ['$rootScope'];
	function profileController($rootScope) {
		$rootScope.pageTitle = "My profile";
	}
})(angular.module('hiTax'));