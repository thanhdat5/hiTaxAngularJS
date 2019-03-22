(function (app) {
	app.controller("invoiceInputController", invoiceInputController);
	invoiceInputController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData', '$state'];
	function invoiceInputController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData, $state) {
		// Set page title
		$rootScope.pageTitle = "Invoice Input";
		$scope.allDepartment = [];
		$scope.allCustomer = [];
		$scope.allCategory = [];
		$scope.allCategoryStored = [];
		$scope.allTaxValue = [];
		$scope.invoices = [];
		$scope.invoiceSelected = {};
		$scope.invoiceSelectedCodeStored = "";
		$scope.invoiceDetail = { DepartmentId: "", CategoryId: "", TaxValueId: "", Value: 0 };
		$scope.userInfo = JSON.parse(sessionStorage.hiTaxUserLoggedInfo);
		$scope.isEditDetail = false;
		$scope.editDetailIndex = 0;

		// Set data dropdownlist
		function loadMasterData() {
			apiService.get('/api/Departments/GetAll', null, function (response) {
				$scope.allDepartment = response.data || [];
			}, function () {
				$scope.allDepartment = [];
			});
			apiService.get('/api/Customers/GetAll', null, function (response) {
				$scope.allCustomer = response.data || [];
			}, function () {
				$scope.allCustomer = [];
			});
			apiService.get('/api/Categories/GetAll', null, function (response) {
				$scope.allCategory = response.data || [];
				_.each($scope.allCategory, function (x, xIndex) {
					if (x.Name.trim().toLowerCase() == 'doanh thu' || x.Name.trim().toLowerCase() == 'gs doanh thu') {
						x.IsShow = false;
					} else {
						x.IsShow = true;
					}
				});
			}, function () {
				$scope.allCategory = [];
			});
			apiService.get('/api/TaxValues/GetAll', null, function (response) {
				$scope.allTaxValue = response.data || [];
			}, function () {
				$scope.allTaxValue = [];
			});
		}
		loadMasterData();

		$scope.$watch("invoiceSelected.IsIn", function (data) {
			_.each($scope.allCategory, function (x, xIndex) {
				if (x.Name.trim().toLowerCase() == 'doanh thu' || x.Name.trim().toLowerCase() == 'gs doanh thu') {
					x.IsShow = !data;
				} else {
					x.IsShow = data;
				}
			});
		});

		// Add new invoice
		$scope.addNewItem = function () {
			var currentDate = new Date();
			var todayDate = kendo.toString(kendo.parseDate(new Date()), 'MM/dd/yyyy');

			var newInvoice = {
				Id: 0,
				Code: "HD" + currentDate.getFullYear() + '' + (currentDate.getMonth() + 1) + '' + (currentDate.getDate()) + '' + currentDate.getHours() + '' + currentDate.getMinutes() + '' + currentDate.getSeconds(),
				CreatedDate: todayDate,
				Value: 0,
				CompanyId: $scope.userInfo.CompanyId,
				CustomerId: '',
				IsIn: true,
				Details: []
			}
			var exists = _.findIndex($scope.invoices, function (x, xIndex) {
				return x.Code == newInvoice.Code;
			});
			if (exists != -1) {
				notificationService.displayWarning("The invoice " + newInvoice.Code + " already exists.");
				return;
			} else {
				$scope.invoices.push(newInvoice);
			}
			$scope.invoiceSelected = $scope.invoices[$scope.invoices.length - 1];
			$scope.invoiceSelectedCodeStored = $scope.invoiceSelected.Code;
		}
		$scope.addNewItem();

		$scope.addInvoiceDetail = function () {
			var validator = $("#sub-form").kendoValidator().data("kendoValidator");
			if (!validator.validate()) {
				return;
			}

			if (!$scope.invoiceSelected.Details) {
				$scope.invoiceSelected.Details = [];
			}

			$scope.invoiceSelected.Details.push({
				Id: 0,
				InvoiceId: 0,
				InvoiceIsIn: $scope.invoiceSelected.IsIn,
				CustomerId: $scope.invoiceSelected.CustomerId,
				CustomerName: $scope.getCustomerNameById($scope.invoiceSelected.CustomerId),
				DepartmentId: $scope.invoiceDetail.DepartmentId,
				DepartmentName: $scope.getDepartmentNameById($scope.invoiceDetail.DepartmentId),
				CategoryId: $scope.invoiceDetail.CategoryId,
				CategoryName: $scope.getCategoryNameById($scope.invoiceDetail.CategoryId),
				Value: $scope.invoiceDetail.Value,
				TaxValueId: $scope.invoiceDetail.TaxValueId,
				ValueTax: $scope.getValueTaxById($scope.invoiceDetail.TaxValueId),
			});
			calculatorTotalValue();
			$scope.cancelInvoiceDetail();
		}

		$scope.getDepartmentNameById = function (id) {
			var exist = _.find($scope.allDepartment, function (x, xIndex) {
				return id == x.Id;
			})
			return exist != null ? exist.DepartmentName : "";
		}
		$scope.getCustomerNameById = function (id) {
			var exist = _.find($scope.allCustomer, function (x, xIndex) {
				return id == x.Id;
			})
			return exist != null ? exist.CustomerName : "";
		}
		$scope.getCategoryNameById = function (id) {
			var exist = _.find($scope.allCategory, function (x, xIndex) {
				return id == x.Id;
			})
			return exist != null ? exist.Name : "";
		}
		$scope.getValueTaxById = function (id) {
			var exist = _.find($scope.allTaxValue, function (x, xIndex) {
				return id == x.Id;
			})
			return exist != null ? (parseFloat(exist.Value) * 100).toFixed(2) + "%" : "";
		}
		function calculatorTotalValue() {
			var total = 0;
			if ($scope.invoiceSelected) {
				_.each($scope.invoiceSelected.Details, function (x, xIndex) {
					total += x.Value;
				});
			}
			$scope.invoiceSelected.Value = total;
		}

		// Select invoice to show details
		$scope.selectInvoice = function (obj) {
			var exists = _.findIndex($scope.invoices, function (x, xIndex) {
				return x.Code == $scope.invoiceSelectedCodeStored;
			});
			if (exists != -1) {
				$scope.invoices[exists] = $scope.invoiceSelected;
			}
			$scope.invoiceSelected = obj;
			$scope.invoiceSelectedCodeStored = $scope.invoiceSelected.Code;
		}

		// Delete this selected invoice
		$scope.deleteItem = function () {
			if ($scope.invoices != null && $scope.invoices.length == 1) {
				return;
			}
			var exists = _.findIndex($scope.invoices, function (x, xIndex) {
				return x.Code == $scope.invoiceSelectedCodeStored;
			});
			if (exists == -1) {
				return;
			} else {
				$scope.invoices.splice(exists, 1);
				notificationService.displaySuccess("The record was removed successfully.");
				$scope.invoiceSelected = $scope.invoices[$scope.invoices.length - 1];
				$scope.invoiceSelectedCodeStored = $scope.invoiceSelected.Code;
			}
		}

		// Save this selected invoice
		$scope.saveItem = function () {
			var validator = $("#main-form").kendoValidator().data("kendoValidator");
			if (!validator.validate()) {
				return;
			}
			//var exists = _.findIndex($scope.invoices, function (x, xIndex) {
			//	return x.Code == $scope.invoiceSelectedCodeStored;
			//});
			//if (exists == -1) {
			//	notificationService.displayWarning("Can not save this invoice.");
			//	return;
			//} else {
				// Call api to save item
				var postData = [];
				postData.push($scope.invoiceSelected);
				apiService.post('/api/invoices/AddV2', postData,
					function (success) {
						$scope.invoices = [];
						$scope.invoiceSelected = {};
						$scope.invoiceSelectedCodeStored = "";
						$scope.deleteItem();
						notificationService.displaySuccess('The invoice was added successfully.');
					},
					function (error) {
						notificationService.displayError(error.data.Message);
						notificationService.displayErrorValidation(error);
					}
				);
			//}
		}

		// Save all invoices
		$scope.saveAllItems = function () {
			// Call api to save item
			var postData = $scope.invoices;
			apiService.post('/api/invoices/AddV2', postData,
				function (success) {
					notificationService.displaySuccess('The invoice was added successfully.');
					$ngBootbox.confirm('Are you want to go to invoices list page?')
						.then(function () {
							$state.go('invoices');
						}, function () {
						});
					$scope.invoices = [];
					$scope.invoiceSelected = {};
					$scope.invoiceSelectedCodeStored = "";
					$scope.addNewItem();
				},
				function (error) {
					notificationService.displayError(error.data.Message);
					notificationService.displayErrorValidation(error);
				}
			);
		}

		$scope.editDetail = function (index) {
			$scope.editDetailIndex = index;
			var item = $scope.invoiceSelected.Details[index];
			$scope.invoiceDetail = { DepartmentId: item.DepartmentId, CategoryId: item.CategoryId, TaxValueId: item.TaxValueId, Value: item.Value };
			$scope.isEditDetail = true;
		}
		$scope.removeDetail = function (index) {
			if (index == -1) {
				return;
			} else {
				$scope.invoiceSelected.Details.splice(index, 1);
				calculatorTotalValue();
				notificationService.displaySuccess("The record was removed successfully.");
			}
		}

		$scope.saveInvoiceDetail = function () {
			var newItem = {
				Id: 0,
				InvoiceId: 0,
				InvoiceIsIn: $scope.invoiceSelected.IsIn,
				CustomerId: $scope.invoiceSelected.CustomerId,
				InvoiceType: $scope.invoiceSelected.IsIn ? "In" : "Out",
				CustomerName: $scope.getCustomerNameById($scope.invoiceSelected.CustomerId),
				DepartmentId: $scope.invoiceDetail.DepartmentId,
				DepartmentName: $scope.getDepartmentNameById($scope.invoiceDetail.DepartmentId),
				CategoryId: $scope.invoiceDetail.CategoryId,
				CategoryName: $scope.getCategoryNameById($scope.invoiceDetail.CategoryId),
				Value: $scope.invoiceDetail.Value,
				TaxValueId: $scope.invoiceDetail.TaxValueId,
				ValueTax: $scope.getValueTaxById($scope.invoiceDetail.TaxValueId),
			};
			$scope.invoiceSelected.Details[$scope.editDetailIndex] = newItem;
			calculatorTotalValue();
			$scope.cancelInvoiceDetail();
		}

		$scope.cancelInvoiceDetail = function () {
			$scope.invoiceDetail = { DepartmentId: "", CategoryId: "", TaxValueId: "", Value: 0 };
			$scope.isEditDetail = false;
			$scope.editDetailIndex = 0;
		}
	}
})(angular.module('hiTax.invoiceInput', ["kendo.directives"]));
