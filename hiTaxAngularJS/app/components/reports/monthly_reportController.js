(function (app) {
	app.controller("monthlyReportController", monthlyReportController);
	monthlyReportController.$inject = ['$scope', '$rootScope', 'apiService', '$ngBootbox', 'notificationService', 'authData'];
	function monthlyReportController($scope, $rootScope, apiService, $ngBootbox, notificationService, authData) {
		// Set page title
		$rootScope.pageTitle = "Monthly Report";
		$scope.month = null;
		$scope.result = {};
		$scope.isSearch = false;

		$scope.monthSelectorOptions = {
			start: "year",
			depth: "year"
		};

		$scope.search = function () {
			if ($scope.month == null) {
				notificationService.displayWarning("Month is required.");
				return;
			}
			var date = new Date($scope.month),
				y = date.getFullYear(),
				m = date.getMonth();
			var fromDate = kendo.toString(kendo.parseDate(new Date(y, m, 1)), 'MM/dd/yyyy');
			var toDate = kendo.toString(kendo.parseDate(new Date(y, m + 1, 0)), 'MM/dd/yyyy');

			apiService.get('/api/Invoices/AccountingReport?fromDate=' + (fromDate != null ? fromDate : '') + "&toDate=" + (toDate != null ? toDate : ''), null, function (response) {

				var totalIn = 0;
				var totalOut = 0;
				_.each(response.data, function (x, xIndex) {
					if (x.CategoryName.trim().toLowerCase() == 'doanh thu') {
						totalOut += (x.Value - (x.Value / (1 + x.ValueTax)));
					} else {
						if (x.CategoryName.trim().toLowerCase() == 'gs doanh thu') {
							totalOut -= (x.Value - (x.Value / (1 + x.ValueTax)));
						} else {
							if (x.CategoryName.trim().toLowerCase() == 'gs er') {
								totalIn -= (x.Value - (x.Value / (1 + x.ValueTax)));
							} else {
								totalIn += (x.Value - (x.Value / (1 + x.ValueTax)));
							}
						}
					}

				});

				$scope.result = {
					TotalIn: totalIn,
					TotalOut: totalOut
				};

				google.charts.load('current', { packages: ['corechart', 'bar'] });
				google.charts.setOnLoadCallback(function drawAnnotations() {
					var data = google.visualization.arrayToDataTable([
						['Type', 'Total'],
						['Thuế đầu ra', totalOut],
						['Thuế đầu vào', totalIn],
						['Thanh toán', (totalOut > totalIn ? totalOut - totalIn : totalIn - totalOut)],
					]);

					var options = {
						title: "Statistics for " + $scope.month
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
			$scope.month = null;
			$scope.result = {};
			$scope.isSearch = false;
		}
	}
})(angular.module('hiTax.monthlyReport', ["kendo.directives"]));
