using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Request
{
	public class InvoiceAccountRequest
	{
		public int Id { get; set; }
		public int AccountNumber { get; set; }
		public bool IsSymmetric { get; set; }
	}
}