(function (app) {
	app.controller("userListController", userListController);
	userListController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function userListController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "User Management";

		// Declare variable
		$scope.allCompany = [];
		$scope.allDepartment = [];

		// Set data dropdownlist
		function loadMasterData() {
			apiService.get('/api/Companies/GetAll', null, function (response) {
				$scope.allCompany = response.data || [];
			}, function () {
				$scope.allCompany = [];
			});
		}
		loadMasterData();

		$scope.companyChange = function () {
			if (!$scope.popupModel.CompanyId || $scope.popupModel.CompanyId == null || $scope.popupModel.CompanyId == "") {
				$scope.allDepartment = [];
			} else {
				apiService.get('/api/Departments/GetByCompanyId?id=' + $scope.popupModel.CompanyId, null, function (response) {
					$scope.allDepartment = response.data || [];
				}, function () {
					$scope.allDepartment = [];
				});
			}
		}

		// Set data gridview
		$scope.mainGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "hiTax_Users.xlsx",
				proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
				filterable: true
			},
			pdf: {
				allPages: true,
				avoidLinks: true,
				paperSize: "A4",
				margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
				landscape: true,
				repeatHeaders: true,
				scale: 0.8,
				fileName: "hiTax_Users.pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/ApplicationUsers/GetAll",
							beforeSend: function (req) {
								req.setRequestHeader('Authorization', 'Bearer ' + authData.authenticationData.accessToken);
							},
							dataType: "json",
						}
				},
				pageSize: 10,
				serverPaging: true,
				serverSorting: true,
				group: {
					field: "CompanyName",
					aggregates: [{
						field: "CompanyName",
						aggregate: "count"
					}]
				}
			},
			sortable: true,
			pageable: true,
			filterable: {
				extra: false
			},
			resizeable: true,
			columns: [
				{
					field: "Id",
					title: "ID",
					hidden: true
				},
				{
					field: "ImagePath",
					title: "#",
					filterable: false,
					template: function (dataItem) {
						return "<img style='height:40px; width:40px;' ng-src='" + (dataItem.ImagePath != null && dataItem.ImagePath != "" ? dataItem.ImagePath : "/Content/images/NoImage.gif") + "' alt='" + dataItem.UserName + "' err-src='/Content/images/NoImage.gif' />";
					},
					width: "60px"
				},
				{
					field: "UserName",
					title: "UserName"
				},
				{
					field: "DisplayName",
					title: "Display Name"
				},
				{
					field: "CompanyName",
					title: "Company"
				},
				{
					field: "DepartmentName",
					title: "Department"
				},
				{
					field: "Roles",
					title: "Roles",
					template: function (dataItem) {
						return dataItem.Roles.join(",");
					}
				},
				{
					title: "Action",
					template: function (dataItem) {
						var isAdmin = dataItem.Roles.indexOf("SPAdmin") == -1;
						return '<span><button type="button" ng-click="showUpdatePermission(dataItem)" class="btn btn-xs btn-info">Update Permission</button>&nbsp;</span><button type="button" ng-click="showChangePasswordModal(dataItem)" class="btn btn-xs btn-warning">Change password</button>&nbsp;<button class="btn btn-primary btn-xs" ng-click="editItem(dataItem)"><span class="fa fa-edit"></span></button>&nbsp;<button class="btn btn-danger btn-xs" ng-click="deleteItem(dataItem)"><span class="fa fa-trash"></span></button>';
					},
					width: "330px"
				}
			]
		};

		$scope.changePassword = function () {
			var validator = $("#sub-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				if ($scope.changePasswordModal.OldPassword.length < 6 || $scope.changePasswordModal.NewPassword.length < 6 || $scope.changePasswordModal.ConfirmPassword.length < 6) {
					notificationService.displayError("The Password must be at least 6 characters long.");
					return;
				}
				if ($scope.changePasswordModal.NewPassword != $scope.changePasswordModal.ConfirmPassword) {
					notificationService.displayError("The new password and confirmation password do not match.");
					return;
				}
				var postData = {
					OldPassword: $scope.changePasswordModal.OldPassword,
					NewPassword: $scope.changePasswordModal.NewPassword,
					ConfirmPassword: $scope.changePasswordModal.ConfirmPassword
				};
				apiService.put('/api/ApplicationUsers/ChangePassword/' + $scope.changePasswordModal.Id, postData,
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

		// Action
		$scope.popupModel = {};
		$scope.changePasswordModal = {};
		$scope.popupTitle = "";
		$scope.addItem = function () {
			$scope.popupTitle = "Add new item";
			showModal();
		}

		$scope.editItem = function (dataItem) {
			$scope.popupTitle = "Edit item";
			dataItem.Age = dataItem != null && dataItem.Age != null ? parseInt(dataItem.Age) : null;
			$scope.popupModel = JSON.parse(JSON.stringify(dataItem));
			$scope.companyChange();
			showModal();
		}

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
					$scope.popupModel.ImagePath = response;
					notificationService.displaySuccess('The profile image was uploaded successfully.');
				});

			});
		});

		$scope.saveItem = function () {
			var validator = $("#main-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				if ($scope.popupModel.PasswordHash != $scope.popupModel.ConfirmPassword) {
					notificationService.displayError("The new password and confirmation password do not match.");
					return;
				}
				$scope.popupModel.Email = $scope.popupModel.UserName;
				if ($scope.popupModel.Id) {
					apiService.put('/api/ApplicationUsers/Update', $scope.popupModel,
						function (success) {
							notificationService.displaySuccess('User info saved successfully.');
							reloadGrid();
							$scope.closeModal();
						},
						function (error) {
							notificationService.displayError(error.data.Message);
							notificationService.displayErrorValidation(error);
						}
					);
				} else {
					apiService.post('/api/ApplicationUsers/Insert', $scope.popupModel,
						function (success) {
							notificationService.displaySuccess('User info saved successfully.');
							reloadGrid();
							$scope.closeModal();
						},
						function (error) {
							notificationService.displayError(error.data);
							notificationService.displayErrorValidation(error);
						}
					);
				}

			}

		}

		$scope.deleteItem = function (dataItem) {
			$ngBootbox.confirm('Are you sure to remove the selected record?')
				.then(function () {
					var config = {
						params: {
							id: dataItem.Id
						}
					}
					apiService.del('/api/ApplicationUsers/delete', config,
						function (response) {
							notificationService.displaySuccess('The record was removed successfully.');
							reloadGrid();
						},
						function (error) {
							notificationService.displayError('The record was not deleted.');
						});
				}, function () {
				});
		}

		// Hide Modal
		$scope.closeModal = function () {
			$scope.popupModel = {};
			$scope.changePasswordModal = {};
			$scope.changePermissionModel = {};
			$scope.popupTitle = "";
			$('#main-modal').modal('hide');
			$('#change-password-modal').modal('hide');
			$('#change-permission-modal').modal('hide');
		}

		function showModal() {
			$('#main-modal').modal('show');
		}
		$scope.showChangePasswordModal = function (dataItem) {
			$scope.changePasswordModal = {
				Id: dataItem.Id,
				UserName: dataItem.UserName,
				OldPassword: null,
				NewPassword: null,
				ConfirmPassword: null
			};
			$('#change-password-modal').modal('show');
		}
		$scope.showUpdatePermission = function (dataItem) {
			$scope.changePermissionModel = {
				Id: dataItem.Id,
				UserName: dataItem.UserName,
				IsSPAdmin: dataItem.Roles.indexOf("SPAdmin") != -1,
				IsDirector: dataItem.Roles.indexOf("Director") != -1,
				IsStaff: dataItem.Roles.indexOf("Staff") != -1,
			};
			$('#change-permission-modal').modal('show');
		}
		function reloadGrid() {
			$('#main-grid').data('kendoGrid').dataSource.read();
		}


		$scope.changePermission = function () {
			var validator = $("#permission-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				var roles = [];
				if ($scope.changePermissionModel.IsSPAdmin) {
					roles.push("SPAdmin");
				}
				if ($scope.changePermissionModel.IsDirector) {
					roles.push("Director");
				}
				if ($scope.changePermissionModel.IsStaff) {
					roles.push("Staff");
				}
				var postData = {
					Id: $scope.changePermissionModel.Id,
					Roles: roles
				};
				apiService.put('/api/ApplicationUsers/UpdateRole', postData,
					function (success) {
						notificationService.displaySuccess('User roles was changed successfully.');
						$scope.closeModal();
						reloadGrid();
					},
					function (error) {
						notificationService.displayError(error.data);
					}
				);
			}
		}

		$scope.chkPermissionClick = function (event, type) {
			event.preventDefault();
			switch (type) {
				case 1: {
					$scope.changePermissionModel.IsSPAdmin = !$scope.changePermissionModel.IsSPAdmin;
					break;
				}
				case 2: {
					$scope.changePermissionModel.IsDirector = !$scope.changePermissionModel.IsDirector;
					break;
				}
				case 3: {
					$scope.changePermissionModel.IsStaff = !$scope.changePermissionModel.IsStaff;
					break;
				}
			}
		}
	}
})(angular.module('hiTax.users', ["kendo.directives"]));
