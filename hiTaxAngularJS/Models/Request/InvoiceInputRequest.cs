using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Request
{
	public class InvoiceInputRequest
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
		public List<InvoiceDetailRequest> Details { get; set; }
	}

	public class InvoiceDetailRequest
	{
		public int Id { get; set; }
		public int InvoiceId { get; set; }
		public string InvoiceCode { get; set; }
		public int CustomerId { get; set; }
		public string CustomerName { get; set; }
		public decimal Value { get; set; }
		public int DepartmentId { get; set; }
		public string DepartmentName { get; set; }
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
		public int TaxValueId { get; set; }
		public string ValueTax { get; set; }
		public bool InvoiceIsIn { get; set; }
		public string InvoiceType { get; set; }
	}
}