(function (app) {
	app.controller("accountReportController", accountReportController);
	accountReportController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function accountReportController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "Accounting Report";
		$scope.fromDate = null;
		$scope.toDate = null;
		$scope.result = {};
		$scope.isSearch = false;

		$scope.search = function () {
			if ($scope.fromDate == null && $scope.toDate == null) {
				notificationService.displayWarning("At least one date field selected.");
				return;
			}
			if (new Date($scope.fromDate) > new Date()) {
				notificationService.displayWarning("From date could not be later than current date.");
				return;
			}
			apiService.get('/api/Invoices/AccountingReport?fromDate=' + ($scope.fromDate != null ? $scope.fromDate : '') + "&toDate=" + ($scope.toDate != null ? $scope.toDate : ''), null, function (response) {
				var lstIn = [];
				var lstOut = [];
				var lstIn = _.each(response.data, function (x, xIndex) {
					if (x.InvoiceIsIn) {
						lstIn.push(x);
					} else {
						lstOut.push(x);
					}
				});

				var totalCosts = 0;
				_.each(lstIn, function (x, xIndex) {
					totalCosts += x.Value;
				})

				var costDetails = [];
				var dataCosts = _.chain(lstIn).groupBy("CategoryName").map(function (v, i) {
					return {
						CategoryName: i,
						items: v
					}
				}).value();
				_.each(dataCosts, function (x, xIndex) {
					var xValue = 0;
					_.each(x.items, function (y, yIndex) {
						xValue += y.Value;
					});
					costDetails.push({
						CategoryName: x.CategoryName,
						TotalValue: xValue
					});
				});


				var revenue19 = 0;
				var revenue00 = 0;
				var revenue70 = 0;
				_.each(lstOut, function (x, xIndex) {
					if (x.ValueTax = 0.19) {
						revenue19 += x.Value;
					}
					if (x.ValueTax = 0.07) {
						revenue70 += x.Value;
					}
					if (x.ValueTax = 0) {
						revenue00 += x.Value;
					}
				});
				$scope.result = {
					TotalCosts: totalCosts,
					CostDetails: costDetails,
					Revenue19: revenue19,
					Revenue00: revenue00,
					Revenue70: revenue70,
					TotalRevenue: (revenue19 + revenue00 + revenue70)
				};


				google.charts.load('current', { packages: ['corechart', 'bar'] });
				google.charts.setOnLoadCallback(function drawAnnotations() {
					var data = google.visualization.arrayToDataTable([
						['Type', 'Total'],
						['Revenue', $scope.result.TotalRevenue],
						['Costs', $scope.result.TotalCosts],
						['Result (vari.)', ($scope.result.TotalRevenue > $scope.result.TotalCosts ? $scope.result.TotalRevenue - $scope.result.TotalCosts : $scope.result.TotalCosts - $scope.result.TotalRevenue)],
					]);

					var options = {
						title: "Statistics from " + ($scope.fromDate || (new Date()).toLocaleDateString()) + " to " + ($scope.toDate || (new Date()).toLocaleDateString())
					};

					var chart = new google.visualization.PieChart(document.getElementById('myChart'));
					chart.draw(data, options);
				});


				$scope.isSearch = true;
			}, function () {
				$scope.result = {};
				$scope.isSearch = true;
			});
		}

		$scope.clearAll = function () {
			$scope.fromDate = null;
			$scope.toDate = null;
			$scope.result = {};
			$scope.isSearch = false;
		}
	}
})(angular.module('hiTax.accountReport', ["kendo.directives"]));
