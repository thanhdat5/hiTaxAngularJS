(function (app) {
	app.controller('rootController', rootController);

	rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'authenticationService', '$http', '$rootScope'];

	function rootController($state, authData, loginService, $scope, authenticationService, $http, $rootScope) {
		$rootScope.pageTitle = "Dashboard";
		authenticationService.getTokenFromLocalStogare().then(function (data) {
			if (data == null || data == undefined) {
				$state.go('login');
			}
		});
		$scope.logOut = function () {
			loginService.logOut();
		}
		$scope.authentication = authData.authenticationData;
		$scope.sideBar = "/app/shared/views/sideBar.html";

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
	}
})(angular.module('hiTax'));