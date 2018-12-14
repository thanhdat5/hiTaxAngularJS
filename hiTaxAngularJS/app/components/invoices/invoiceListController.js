(function (app) {
	app.controller("invoiceListController", invoiceListController);
	invoiceListController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData', '$rootScope'];
	function invoiceListController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData, $rootScope) {
		// Set page title
		$rootScope.pageTitle = "invoice Management";
		$scope.userInfo = JSON.parse(sessionStorage.hiTaxUserLoggedInfo);
		// Set data gridview
		$scope.mainGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "Invoices List.xlsx",
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
				fileName: "Invoices List.pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/invoices/GetAll",
							beforeSend: function (jqXHR, settings) {
								jqXHR.setRequestHeader('Authorization', 'Bearer ' + authData.authenticationData.accessToken);
							},
							dataType: "json",
						}
				},
				schema: {
					model: {
						fields: {
							CreatedDate: { type: "date" },
							Value: { type: "number" }
						}
					}
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false,
				group: [
					{ field: "CompanyName" },
					{ field: "CreatedDate" }
				]
			},
			sortable: true,
			pageable: true,
			resizeable: true,
			filterable: {
				extra: false,
				operators: {
					date: {
						gt: "After",
						lt: "Before"
					}
				}
			},
			columns: [
				{
					field: "Id",
					title: "ID",
					hidden: true
				},
				{
					field: "Code",
					title: "Code",
				},
				{
					field: "CompanyName",
					title: "Company",
					//hidden: !$rootScope.isSPAdmin
				},
				{
					field: "InvoiceType",
					title: "Type",
					width: '80px'
				},
				{
					field: "Value",
					title: "Total Value",
					type: 'number',
					template: "#= kendo.parseFloat(Value) #",
					width: '110px'
				},
				{
					field: "SH",
					title: "S/H",
					width: '80px',
					hidden: (!$scope.userInfo.IsSPAdmin)
				},
				{
					field: "AccountNumber",
					title: "Account",
					hidden: (!$scope.userInfo.IsSPAdmin),
					template: function (dataItem) {
						return (dataItem.AccountNumber == 0 || dataItem.AccountNumber == "0" ? "" : dataItem.AccountNumber);
					}
				},
				{
					field: "SAccountNumber",
					title: "S.Account",
					hidden: (!$scope.userInfo.IsSPAdmin),
					template: function (dataItem) {
						return (dataItem.SAccountNumber == 0 || dataItem.SAccountNumber == "0" ? "" : dataItem.SAccountNumber);
					}
				},
				{
					field: "CreatedDate",
					title: "Date",
					type: 'date',
					template: "#= kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
					groupHeaderTemplate: "Date: #= kendo.toString(kendo.parseDate(value, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
					filterable: {
						ui: "datetimepicker"
					}
				},
				{
					title: "Action",
					template: function (dataItem) {
						if ($scope.userInfo.IsSPAdmin) {
							return '<div class="text-center"><button class="btn btn-primary btn-xs" ng-click="editItem(dataItem)"><span class="fa fa-pencil"></span></button>&nbsp;<button class="btn btn-danger btn-xs" ng-click="deleteItem(dataItem)"><span class="fa fa-trash"></span></button></div>';
						}
						return '<div class="text-center"><button class="btn btn-danger btn-xs" ng-click="deleteItem(dataItem)"><span class="fa fa-trash"></span></button></div>';
					},
					width: "80px"
				}
			]
		};

		$scope.detailGridOptions = function (dataItem) {
			return {
				dataSource: {
					type: "json",
					transport: {
						read: {
							url: "api/invoices/GetDetailsById?id=" + dataItem.Id,
							beforeSend: function (req) {
								req.setRequestHeader('Authorization', 'Bearer ' + authData.authenticationData.accessToken);
							},
							dataType: "json",
						}
					},
					serverPaging: false,
					serverSorting: false,
					serverFiltering: false,
					pageSize: 10
				},
				schema: {
					model: {
						fields: {
							Value: { type: "number" }
						}
					}
				},
				filterable: {
					extra: false,
					operators: {
						filterable: {
							extra: false
						},
					}
				},
				scrollable: false,
				sortable: true,
				pageable: true,
				resizeable: true,
				columns: [
					{ field: "CustomerName", title: "Customer" },
					{
						field: "ValueTax",
						title: "Tax Value",
						type: 'number',
						template: function (dataItem) {
							var temp = dataItem != null ? (dataItem.ValueTax * 100) + "%" : "0%";
							return temp;
						}
					},
					{ field: "DepartmentName", title: "Department" },
					{ field: "CategoryName", title: "Category" },
					{
						field: "Value",
						type: 'number',
						title: "Bruto",
						template: "#= kendo.parseFloat(Value) #",
					}
				]
			};
		};

		$scope.deleteItem = function (dataItem) {
			$ngBootbox.confirm('Are you sure to remove the selected record?')
				.then(function () {
					var config = {
						params: {
							id: dataItem.Id
						}
					}
					apiService.del('/api/invoices/delete', config,
						function (response) {
							notificationService.displaySuccess('The record was removed successfully.');
							reloadGrid();
							reloadGridModal();
						},
						function (error) {
							notificationService.displayError('The record was not deleted.');
						});
				}, function () {
				});
		}

		function reloadGrid() {
			$('#main-grid').data('kendoGrid').dataSource.read();
			$('#main-grid').data('kendoGrid').refresh();
		}

		function reloadGridModal() {
			$('#modal-grid').data('kendoGrid').dataSource.read();
			$('#modal-grid').data('kendoGrid').refresh();
		}

		// Set data gridview
		$scope.modalGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "Invoices Deleted.xlsx",
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
				fileName: "Invoices Deleted.pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/invoices/GetAllDeleted",
							beforeSend: function (jqXHR, settings) {
								jqXHR.setRequestHeader('Authorization', 'Bearer ' + authData.authenticationData.accessToken);
							},
							dataType: "json",
						}
				},
				schema: {
					model: {
						fields: {
							CreatedDate: { type: "date" },
							Value: { type: "number" }
						}
					}
				},
				pageSize: 10,
				serverPaging: false,
				serverSorting: false,
				group: [
					{ field: "CompanyName" },
					{ field: "CreatedDate" }
				]
			},
			sortable: true,
			pageable: true,
			resizeable: true,
			filterable: {
				extra: false,
				operators: {
					date: {
						gt: "After",
						lt: "Before"
					}
				}
			},
			columns: [
				{
					field: "Id",
					title: "ID",
					hidden: true
				},
				{
					field: "Code",
					title: "Code",
				},
				{
					field: "CompanyName",
					title: "Company",
					//hidden: !$rootScope.isSPAdmin
				},
				{
					field: "InvoiceType",
					title: "Type",
				},
				{
					field: "Value",
					title: "Total Value",
					type: 'number',
					template: "#= kendo.parseFloat(Value) #"
				},
				{
					field: "CreatedDate",
					title: "Created Date",
					type: 'date',
					template: "#= kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
					groupHeaderTemplate: "Date: #= kendo.toString(kendo.parseDate(value, 'yyyy-MM-dd'), 'MM/dd/yyyy') #",
					filterable: {
						ui: "datetimepicker"
					}
				},
				{
					title: "Revert",
					template: function (dataItem) {
						return '<div class="text-center"><button class="btn btn-success btn-xs" ng-click="revertItem(dataItem)"><span class="fa fa-refresh"></span></button></div>';
					},
					width: "70px"
				}
			]
		};

		$scope.revertItem = function (dataItem) {
			$ngBootbox.confirm('Are you sure to revert the selected record?')
				.then(function () {
					var config = {
						params: {
							id: dataItem.Id
						}
					}
					apiService.del('/api/invoices/revertdelete', config,
						function (response) {
							notificationService.displaySuccess('The record was reverted successfully.');
							reloadGrid();
							reloadGridModal();
						},
						function (error) {
							notificationService.displayError('The record was not revert.');
						});
				}, function () {
				});
		}


		// Update Invoice Account and Invoice S/H
		$scope.popupModel = {};
		$scope.allInvoiceAccount = [];
		$scope.allInvoiceSymmetricAccount = [];

		// Set data dropdownlist
		function loadMasterData() {
			apiService.get('/api/InvoiceAcounts/GetInvoiceAccount', null, function (response) {
				$scope.allInvoiceAccount = response.data || [];
			}, function () {
				$scope.allInvoiceAccount = [];
			});
			apiService.get('/api/InvoiceAcounts/GetInvoiceSymmetricAccount', null, function (response) {
				$scope.allInvoiceSymmetricAccount = response.data || [];
			}, function () {
				$scope.allInvoiceSymmetricAccount = [];
			});
		}
		loadMasterData();

		// Form action
		$scope.editItem = function (dataItem) {
			$scope.popupModel = JSON.parse(JSON.stringify(dataItem));
			showModal();
		}
		$scope.saveItem = function () {
			var validator = $("#main-form").kendoValidator().data("kendoValidator");
			if (validator.validate()) {
				if ($scope.popupModel.Id) {
					var postData = {
						Id: $scope.popupModel.Id,
						SH: $scope.popupModel.SH,
						InvoiceAccount_Id: Number($scope.popupModel.InvoiceAccount_Id),
						InvoiceSAccount_Id: Number($scope.popupModel.InvoiceSAccount_Id)
					}
					apiService.put('/api/Invoices/UpdateAccoundAndSH', postData,
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
				}
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
})(angular.module('hiTax.invoices', ["kendo.directives"]));
