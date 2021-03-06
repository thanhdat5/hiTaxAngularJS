﻿(function (app) {
	app.controller("invoiceListController", invoiceListController);
	invoiceListController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData', '$rootScope','$http'];
	function invoiceListController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData, $rootScope, $http) {
		// Set page title
		$rootScope.pageTitle = "invoice Management";
		$scope.userInfo = JSON.parse(sessionStorage.hiTaxUserLoggedInfo);

		$scope.showDropdown = false;
		var xDate = new Date();
		$scope.filter = "Current Year";
		$scope.fromDate = new Date(xDate.getFullYear(), 0, 1);
		$scope.toDate = new Date(xDate.getFullYear(), 11, 31);
		$scope.defaultFilter = "year";

		// Filter by date
		$scope.filterDate = function (startDate, endDate, period) {
			$scope.fromDate = startDate._d;
			$scope.toDate = endDate._d;

			$scope.defaultFilter = period;
			var month = $scope.fromDate.getMonth() + 1;
			var year = $scope.fromDate.getFullYear();
			switch (period) {
				case 'month': {
					$scope.toDate = new Date(year, month, 0);
					$scope.filter = month + "/" + year;
					break;
				}
				case 'quarter': {
					var quarter = getQuarter($scope.fromDate);
					$scope.toDate = new Date(year, quarter * 3, 0);
					$scope.filter = "Quarter " + quarter;
					break;
				}
				case 'year': {
					$scope.toDate = new Date(year, 12, 0);
					$scope.filter = "Year " + year;
					if (year == xDate.getFullYear()) {
						$scope.filter = "Current Year";
					}
					break;
				}
				default:
			}

			// Reload data
			reloadGrid();
			reloadGridModal();

			$scope.$apply(function () {
				$scope.showDropdown = false;
			});
		}

		$scope.removeFilter = function (type) {
			$scope.fromDate = new Date(1, 1, 1900);
			$scope.toDate = new Date();
			$scope.defaultFilter = "year";
			$scope.filter = "All";
			$scope.showDropdown = false;

			// Reload data
			reloadGrid();
			reloadGridModal();
			$(".range_filter").find('.start-date:first').removeClass('start-date');
		}

		function getQuarter(date) {
			var quarter = Math.floor((date.getMonth() + 3) / 3);
			return quarter;
		}

		// Set data gridview
		$scope.mainGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "hiTax_Invoices.xlsx",
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
				fileName: "hiTax_Invoices.pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/invoices/GetAll",
							beforeSend: function (jqXHR, settings) {
								settings.url = "api/invoices/GetAll?fromDate=" + formatDate($scope.fromDate) + "&toDate=" + formatDate($scope.toDate);
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
					template: "#= (kendo.parseFloat(Value)).toFixed(2) + ' €' #",
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
						template: "#= kendo.parseFloat(Value).toFixed(2) + ' €' #",
					},
					{
						field: "Value",
						type: 'number',
						title: "Value",
						template: "#= (kendo.parseFloat(Value)/(1 + kendo.parseFloat(ValueTax))).toFixed(2) + ' €' #",
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
				fileName: "hiTax_Invoices Deleted.xlsx",
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
				fileName: "hiTax_Invoices Deleted.pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/invoices/GetAllDeleted",
							beforeSend: function (jqXHR, settings) {
								settings.url = "api/invoices/GetAllDeleted?fromDate=" + formatDate($scope.fromDate) + "&toDate=" + formatDate($scope.toDate);
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

		function formatDate(dateInput) {
			return kendo.toString(kendo.parseDate(dateInput, 'yyyy-MM-dd'), 'MM/dd/yyyy');
		}

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




		//Regular Accounting
		// Set data gridview
		$scope.regularAccountingGridOptions = {
			toolbar: ["excel", "pdf"],
			excel: {
				fileName: "hiTax_Regular Accounting_" + $scope.filter + ".xlsx",
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
				fileName: "hiTax_Regular Accounting_" + $scope.filter + ".pdf",
			},
			dataSource: {
				type: "json",
				transport: {
					read:
						{
							url: "api/invoices/GetAll",
							beforeSend: function (jqXHR, settings) {
								settings.url = "api/invoices/GetAll?fromDate=" + formatDate($scope.fromDate) + "&toDate=" + formatDate($scope.toDate);
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
					},
					hidden: true
				}
			]
		};

		$scope.detailTaxAccountingGridOptions = function (dataItem) {
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
						template: "#= kendo.parseFloat(Value).toFixed(2) + ' €' #",
					},
					{
						field: "Value",
						type: 'number',
						title: "Value",
						template: "#= (kendo.parseFloat(Value) - (kendo.parseFloat(Value)/(1 + kendo.parseFloat(ValueTax)))).toFixed(2) + ' €' #",
					}
				]
			};
		};

		$scope.detailDebtAccountingGridOptions = function (dataItem) {
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
						template: "#= kendo.parseFloat(Value).toFixed(2) + ' €' #",
					},
					{
						field: "Value",
						type: 'number',
						title: "Value",
						template: "#= kendo.parseFloat(Value).toFixed(2) + ' €' #",
					}
				]
			};
		};

		$scope.exportAccounting = function () {
			$http.defaults.headers.post['Authorization'] = 'Bearer ' + authData.authenticationData.accessToken;
			$http.post("/api/Invoices/ExportAccounting?fromDate=" + formatDate($scope.fromDate) + "&toDate=" + formatDate($scope.toDate), null,
				{
					responseType: "arraybuffer"
				}).then(function (response) {
					var myBuffer = new Uint8Array(response.data);
					var data = new Blob([myBuffer],
						{ type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
					var fileName = "hiTax_Export Accounting_" + moment().format('DDMMYYYY') + ".xlsx";
					saveAs(data, fileName);
				});
		}
	}
})(angular.module('hiTax.invoices', ["kendo.directives"]));
