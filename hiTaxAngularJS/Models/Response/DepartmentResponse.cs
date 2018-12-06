using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class DepartmentResponse
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public string DepartmentName { get; set; }
		public string Address { get; set; }
	}
}