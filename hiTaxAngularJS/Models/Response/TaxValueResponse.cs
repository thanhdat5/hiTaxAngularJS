using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class TaxValueResponse
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public double Value { get; set; }
	}
}