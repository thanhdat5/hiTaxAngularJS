(function (app) {
    app.controller('rootController', rootController);

    rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'authenticationService'];

	function rootController($state, authData, loginService, $scope, authenticationService) {
		var token = authenticationService.getTokenInfo();
		if (!token) {
			$state.go('login');
		}
        $scope.logOut = function () {
            loginService.logOut();
        }
        $scope.authentication = authData.authenticationData;
        $scope.sideBar = "/app/shared/views/sideBar.html";
    }
})(angular.module('hiTax'));