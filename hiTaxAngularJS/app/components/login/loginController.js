(function (app) {
	app.controller('loginController', ['$scope', 'loginService', '$injector', 'notificationService', 'authenticationService', '$state',
		function ($scope, loginService, $injector, notificationService, authenticationService, $state) {
			authenticationService.getTokenFromLocalStogare().then(function (data) {
				if (data != null && data != undefined) {
					$state.go('admin');
				}
			});
			$scope.loginData = {
				userName: "",
				password: ""
			};

			$scope.loginSubmit = function () {
				loginService.login($scope.loginData.userName, $scope.loginData.password).then(
					function (response) {
						if (response != null) {
							notificationService.displayError("[Internal Server Error] " + response.statusText);
						}
						else {
							var stateService = $injector.get('$state');
							stateService.go('admin');
						}
					}, function (error) {
						notificationService.displayError("Username or password is invalid.");
					});
			}
		}]);
})(angular.module('hiTax'));