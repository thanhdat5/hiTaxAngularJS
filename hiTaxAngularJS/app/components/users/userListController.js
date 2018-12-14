(function (app) {
	app.controller("userListController", userListController);
	userListController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function userListController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "User Management";

		// Declare variable
		$scope.allCompany = [];

		// Set data dropdownlist
		function loadMasterData() {
			apiService.get('/api/Companies/GetAll', null, function (response) {
				$scope.allCompany = response.data || [];
			}, function () {
				$scope.allCompany = [];
			});
		}
		loadMasterData();

		// Set data gridview
		$scope.mainGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "Users List.xlsx",
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
				fileName: "Users List.pdf",
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
				}, {
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
					field: "Roles",
					title: "Roles",
					template: function (dataItem) {
						return dataItem.Roles.join(",");
					}
				},
				{
					title: "Action",
					template: function (dataItem) {
						return '<button class="btn btn-primary btn-xs" ng-click="editItem(dataItem)"><span class="fa fa-edit"></span></button>&nbsp;<button class="btn btn-danger btn-xs" ng-click="deleteItem(dataItem)"><span class="fa fa-trash"></span></button>';
					},
					width: "80px"
				}
			]
		};

		// Action
		$scope.popupModel = {};
		$scope.popupTitle = "";
		$scope.addItem = function () {
			$scope.popupTitle = "Add new item";
			showModal();
		}

		$scope.editItem = function (dataItem) {
			$scope.popupTitle = "Edit item";
			$scope.popupModel = JSON.parse(JSON.stringify(dataItem));
			showModal();
		}

		$scope.saveItem = function () {
			if ($scope.popupModel.Id) {
				alert("Edit");
			} else {
				alert("Add");
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
			$scope.popupTitle = "";
			$('#main-modal').modal('hide');
		}

		function showModal() {
			$('#main-modal').modal('show');
		}

		function reloadGrid() {
			$('#main-grid').data('kendoGrid').dataSource.read();
		}
	}
})(angular.module('hiTax.users', []));
