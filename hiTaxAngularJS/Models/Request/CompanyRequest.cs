using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Request
{
	public class CompanyRequest
	{
		public int Id { get; set; }
		public string CompanyName { get; set; }
		public string Description { get; set; }
	}
}