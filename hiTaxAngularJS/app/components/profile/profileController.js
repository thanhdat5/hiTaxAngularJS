
(function (app) {
	app.controller("profileController", profileController);
	profileController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function profileController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "My Profile";

		// Declare variable
		$scope.allCompany = [];
		$scope.userInfo = {};

		// Get user info
		apiService.get('/api/ApplicationUsers/GetProfile', null, function (response) {
			$scope.userInfo = response.data;
			sessionStorage.hiTaxUserLoggedInfo = JSON.stringify(response.data);
			authData.authenticationData.UserInfo = $scope.userInfo;
			$rootScope.userLoggedDisplayName = authData.authenticationData.UserInfo.DisplayName;
			try {
				$scope.userInfo.Age = parseInt($scope.userInfo.Age);
			} catch (e) {

			}
		}, function () {
			$scope.userInfo = {};
			sessionStorage.hiTaxUserLoggedInfo = null;
		});

		// Get master data
		apiService.get('/api/Companies/GetAll', null, function (response) {
			$scope.allCompany = response.data || [];
		}, function () {
			$scope.allCompany = [];
		});

		// Select image
		$scope.selectFile = function () {
			$('#uploadEditorImage')[0].value = null;
			$('#uploadEditorImage').trigger("click");
		}

		// Upload image
		$("#uploadEditorImage").change(function () {
			var data = new FormData();
			var files = $("#uploadEditorImage").get(0).files;
			if (files.length > 0) {
				data.append("UploadedImage", files[0]);
			}
			var ajaxRequest = $.ajax({
				type: "POST",
				url: "/api/ApplicationUsers/UploadImage",
				contentType: false,
				processData: false,
				data: data,
				beforeSend: function (xhr) {
					xhr.setRequestHeader("Authorization", 'Bearer ' + authData.authenticationData.accessToken);
				},
			});

			ajaxRequest.done(function (response, textStatus) {
				$scope.$apply(function () {
					$scope.userInfo.ImagePath = response;
					notificationService.displaySuccess('The profile image was uploaded successfully.');
				});

			});
		});

		// Save info
		$scope.saveChange = function () {
			var validator = $("#main-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				apiService.put('/api/ApplicationUsers/Update', $scope.userInfo,
					function (success) {
						notificationService.displaySuccess('Profile saved successfully.');
					},
					function (error) {
						notificationService.displayError(error.data.Message);
						notificationService.displayErrorValidation(error);
					}
				);
			}
		}

		$scope.popupModel = {};
		$scope.changePassword = function (dataItem) {
			$scope.popupModel = {};
			showModal();
		}

		$scope.saveItem = function () {
			var validator = $("#sub-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				if ($scope.popupModel.OldPassword.length < 6 || $scope.popupModel.NewPassword.length < 6 || $scope.popupModel.ConfirmPassword.length < 6) {
					notificationService.displayError("The Password must be at least 6 characters long.");
					return;
				}
				if ($scope.popupModel.NewPassword != $scope.popupModel.ConfirmPassword) {
					notificationService.displayError("The new password and confirmation password do not match.");
					return;
				}
				apiService.put('/api/ApplicationUsers/ChangePassword', $scope.popupModel,
					function (success) {
						notificationService.displaySuccess('Password was changed successfully.');
						$scope.closeModal();
					},
					function (error) {
						notificationService.displayError(error.data);
					}
				);
			}
		}

		// Hide Modal
		$scope.closeModal = function () {
			$scope.popupModel = {};
			$('#main-modal').modal('hide');
		}

		function showModal() {
			$('#main-modal').modal('show');
		}
	}
})(angular.module('hiTax.profile', ["kendo.directives"]));
