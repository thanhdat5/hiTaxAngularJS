(function (app) {
	app.controller('rootController', rootController);

	rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'authenticationService', '$http', '$rootScope','apiService'];

	function rootController($state, authData, loginService, $scope, authenticationService, $http, $rootScope, apiService) {
		$rootScope.pageTitle = "Dashboard";
		authenticationService.getTokenFromLocalStogare().then(function (data) {
			if (data == null || data == undefined) {
				$state.go('login');
			}
		});
		$scope.logOut = function () {
			loginService.logOut();
		}
		// Get user info
		apiService.get('/api/ApplicationUsers/GetProfile', null, function (response) {
			authData.authenticationData.UserInfo = response.data;
		}, function () {
			authData.authenticationData.UserInfo = {};
		});
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