(function (app) {
	app.controller('rootController', rootController);

	rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'authenticationService', '$http', '$rootScope', 'apiService'];

	function rootController($state, authData, loginService, $scope, authenticationService, $http, $rootScope, apiService) {
		$rootScope.pageTitle = "Dashboard";
		$rootScope.currentState = $state.current.name;
		$rootScope.isSPAdmin = false;
		$rootScope.isDirector = false;
		$rootScope.isStaff = false;

		$scope.$watch(function () {
			$rootScope.currentState = $state.current.name;
		});

		authenticationService.getTokenFromLocalStogare().then(function (data) {
			if (data == null || data == undefined) {
				sessionStorage.hiTaxUserLoggedInfo = null;
				$state.go('login');
			} else {

			}
		});

		// Get user info
		apiService.get('/api/ApplicationUsers/GetProfile', null, function (response) {
			if (response) {
				authData.authenticationData.UserInfo = response.data;
				$rootScope.userLoggedDisplayName = response.data.DisplayName;
				sessionStorage.hiTaxUserLoggedInfo = JSON.stringify(response.data);

				var currentRoles = response.data.Roles.join(',');
				$rootScope.isSPAdmin = currentRoles.indexOf("SPAdmin") != -1;
				$rootScope.isDirector = currentRoles.indexOf("Director") != -1;
				$rootScope.isStaff = currentRoles.indexOf("Staff") != -1;
			} else {
				authData.authenticationData.UserInfo = {};
				sessionStorage.hiTaxUserLoggedInfo = null;
				localStorage.clear();
				$state.go('login');
			}
		}, function () {
			authData.authenticationData.UserInfo = {};
			sessionStorage.hiTaxUserLoggedInfo = null;
			localStorage.clear();
			$state.go('login');
		});

		$scope.logOut = function () {
			loginService.logOut();
			sessionStorage.hiTaxUserLoggedInfo = null;
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