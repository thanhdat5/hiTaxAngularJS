using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hiTaxAngularJS.Models.Response
{
	public class ApplicationUserResponse:ApplicationUser
	{
		public string[] Roles { get; set; }
	}
}