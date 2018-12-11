using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class InvoiceAccountResponse
	{
		public int Id { get; set; }
		public int AccountNumber { get; set; }
		public bool IsSymmetric { get; set; }
	}
}