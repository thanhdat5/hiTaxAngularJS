using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class CustomerResponse
	{
		public int Id { get; set; }
		public string CustomerName { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public int CustomerTypeId { get; set; }
		public string CustomerTypeName { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
	}
}