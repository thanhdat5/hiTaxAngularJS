using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class InvoiceResponse
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public DateTime CreatedDate { get; set; }
		public decimal Value { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public int CustomerId { get; set; }
		public string CustomerName { get; set; }
		public bool IsIn { get; set; }
		public string InvoiceType { get; set; }
		public string Date { get; set; }
		public string SH { get; set; }
		public int? InvoiceAccount_Id { get; set; }
		public int? InvoiceSAccount_Id { get; set; }
		public int? AccountNumber { get; set; }
		public int? SAccountNumber { get; set; }
	}
	public class InvoiceExportResponse:InvoiceResponse{
		public int DepartmentId { get; set; }
		public string DepartmentName { get; set; }
		public double ValueTax { get; set; }
	}
}