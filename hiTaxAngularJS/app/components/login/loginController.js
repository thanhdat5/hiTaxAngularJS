(function (app) {
	app.controller('loginController', ['$scope', 'loginService', '$injector', 'notificationService', 'authenticationService', '$state','$http',
		function ($scope, loginService, $injector, notificationService, authenticationService, $state, $http) {
			$scope.isLoading = function () {
				return $http.pendingRequests.length > 0;
			};

			$scope.$watch($scope.isLoading, function (v) {
				if (v) {
					$('.loading-spiner-holder').show();
				} else {
					$('.loading-spiner-holder').hide();
				}
			});

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
							//var stateService = $injector.get('$state');
							//stateService.go('admin');
							window.location.reload();
						}
					}, function (error) {
						notificationService.displayError("Username or password is invalid.");
					});
			}
		}]);
})(angular.module('hiTax'));