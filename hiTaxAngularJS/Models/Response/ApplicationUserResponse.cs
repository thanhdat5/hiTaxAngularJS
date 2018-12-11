using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class ApplicationUserResponse
	{
		public string Id { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public string DisplayName { get; set; }
		public string ImagePath { get; set; }
		public string Address { get; set; }
		public string Age { get; set; }
		public string AboutMe { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string UserName { get; set; }
		public List<string> Roles { get; set; }
		public bool IsSPAdmin { get; set; }
		public bool IsDirector { get; set; }
		public bool IsStaff { get; set; }
	}
}