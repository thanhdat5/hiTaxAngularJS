using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Request
{
	public class ProductRequest
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public string ProductName { get; set; }
		public string Description { get; set; }
		public int UnitId { get; set; }
		public string UnitName { get; set; }
	}
}