/// <reference path="../content/template/libs/angular/angular.js" />

(function () {
	angular.module('hiTax',
		[
			'hiTax.users',
			'hiTax.profile',
			'hiTax.companies',
			'hiTax.categories',
			'hiTax.units',
			'hiTax.customertypes',
			'hiTax.departments',
			'hiTax.customers',
			'hiTax.taxvalues',
			'hiTax.products',
			'hiTax.invoices',
			'hiTax.accountReport',
			'hiTax.monthlyReport',
			'hiTax.invoiceInput',
			'hiTax.accounts',
			'hiTax.common'
		]
	)
		.config(config)
		.config(configAuthentication);

	config.$inject = ['$stateProvider', '$urlRouterProvider'];

	function config($stateProvider, $urlRouterProvider) {
		$stateProvider
			.state('base', {
				url: '',
				controller: "rootController",
				templateUrl: '/app/shared/views/baseView.html',
				abstract: true
			})
			.state('login', {
				url: "/login",
				templateUrl: "/app/components/login/loginView.html",
				controller: "loginController"
			})
			.state('profile', {
				url: "/profile",
				parent: 'base',
				templateUrl: "/app/components/profile/profileView.html",
				controller: "profileController"
			})
			.state('admin', {
				url: "/admin",
				parent: 'base',
				templateUrl: "/app/components/home/homeView.html",
				controller: "homeController"
			})
			.state('configs', {
				url: "/configs",
				parent: 'base',
				templateUrl: "/app/components/configs/configsView.html",
				controller: "configsController"
			})
			.state('applicationusers', {
				url: "/users",
				parent: 'base',
				templateUrl: "/app/components/users/userListView.html",
				controller: "userListController"
			})
			.state('companies', {
				url: "/companies",
				parent: 'base',
				templateUrl: "/app/components/companies/companyListView.html",
				controller: "companyListController"
			})
			.state('categories', {
				url: "/categories",
				parent: 'base',
				templateUrl: "/app/components/categories/categoryListView.html",
				controller: "categoryListController"
			})
			.state('units', {
				url: "/units",
				parent: 'base',
				templateUrl: "/app/components/units/unitListView.html",
				controller: "unitListController"
			})
			.state('customertypes', {
				url: "/customertypes",
				parent: 'base',
				templateUrl: "/app/components/customertypes/customertypeListView.html",
				controller: "customertypeListController"
			})
			.state('departments', {
				url: "/departments",
				parent: 'base',
				templateUrl: "/app/components/departments/departmentListView.html",
				controller: "departmentListController"
			})
			.state('customers', {
				url: "/customers",
				parent: 'base',
				templateUrl: "/app/components/customers/customerListView.html",
				controller: "customerListController"
			})
			.state('taxvalues', {
				url: "/taxvalues",
				parent: 'base',
				templateUrl: "/app/components/taxvalues/taxvalueListView.html",
				controller: "taxvalueListController"
			})
			.state('products', {
				url: "/products",
				parent: 'base',
				templateUrl: "/app/components/products/productListView.html",
				controller: "productListController"
			})
			.state('invoices', {
				url: "/invoices",
				parent: 'base',
				templateUrl: "/app/components/invoices/invoiceListView.html",
				controller: "invoiceListController"
			})
			.state('account_report', {
				url: "/account_report",
				parent: 'base',
				templateUrl: "/app/components/reports/account_reportView.html",
				controller: "accountReportController"
			})
			.state('monthly_report', {
				url: "/monthly_report",
				parent: 'base',
				templateUrl: "/app/components/reports/monthly_reportView.html",
				controller: "monthlyReportController"
			})
			.state('invoice_input', {
				url: "/invoice_input",
				parent: 'base',
				templateUrl: "/app/components/invoiceInput/invoiceInputView.html",
				controller: "invoiceInputController"
			})
			.state('accounts', {
				url: "/accounts",
				parent: 'base',
				templateUrl: "/app/components/accounts/accountListView.html",
				controller: "accountListController"
			})
			;
		$urlRouterProvider.otherwise('/login');
	}

	function configAuthentication($httpProvider) {
		$httpProvider.interceptors.push(function ($q, $location) {
			return {
				request: function (config) {
					return config;
				},
				requestError: function (rejection) {

					return $q.reject(rejection);
				},
				response: function (response) {
					if (response.status == "401") {
						$location.path('/login');
					}
					//the same response/modified/or a new one need to be returned.
					return response;
				},
				responseError: function (rejection) {

					if (rejection.status == "401") {
						$location.path('/login');
					}
					return $q.reject(rejection);
				}
			};
		});
	}
})();