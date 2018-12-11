(function (app) {
	app.controller("productListController", productListController);
	productListController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function productListController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "Product Management";

		// Declare variable
		$scope.allCompany = [];
		$scope.allUnit = [];

		// Set data dropdownlist
		function loadMasterData() {
			apiService.get('/api/Companies/GetAll', null, function (response) {
				$scope.allCompany = response.data || [];
			}, function () {
				$scope.allCompany = [];
			});
			apiService.get('/api/Units/GetAll', null, function (response) {
				$scope.allUnit = response.data || [];
			}, function () {
				$scope.allUnit = [];
			});
		}
		loadMasterData();

		// Set data gridview
		$scope.mainGridOptions = {
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/products/GetAll",
							beforeSend: function (req) {
								req.setRequestHeader('Authorization', 'Bearer ' + authData.authenticationData.accessToken);
							},
							dataType: "json",
						}
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false,
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
					field: "CompanyId",
					title: "Company ID",
					hidden: true
				},
				{
					field: "UnitId",
					title: "Unit ID",
					hidden: true
				},
				{
					field: "CompanyName",
					title: "Company"
				},
				{
					field: "ProductName",
					title: "Product Name"
				},
				{
					field: "UnitName",
					title: "Unit"
				},
				{
					field: "Description",
					title: "Description"
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
			var validator = $("#main-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				if ($scope.popupModel.Id) {
					apiService.put('/api/products/Update', $scope.popupModel,
						function (success) {
							notificationService.displaySuccess('The record was saved successfully.');
							reloadGrid();
							$scope.closeModal();
						},
						function (error) {
							notificationService.displayError(error.data.Message);
							notificationService.displayErrorValidation(error);
						}
					);
				} else {
					$scope.popupModel.Id = 0;
					apiService.post('/api/products/Add', $scope.popupModel,
						function (success) {
							notificationService.displaySuccess('The record was added successfully.');
							reloadGrid();
							$scope.closeModal();
						},
						function (error) {
							notificationService.displayError(error.data.Message);
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
					apiService.del('/api/products/delete', config,
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
})(angular.module('hiTax.products', ["kendo.directives"]));
