using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Request
{
	public class InvoiceRequest
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
	}

	public class InvoiceAcountRequest
	{
		public int Id { get; set; }
		public int InvoiceAccount_Id { get; set; }
		public int InvoiceSAccount_Id { get; set; }
		public string SH { get; set; }
	}
}