using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using hiTax.Web;
using hiTaxAngularJS.Models;
using hiTaxAngularJS.Models.Request;
using hiTaxAngularJS.Models.Response;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;

namespace hiTaxAngularJS.Common
{
	public class PermissionHelper : ApiController
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();

		public PermissionHelper()
		{
		}
		public PermissionHelper(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}
		public ApplicationUserResponse GetUserInfo()
		{
			var currentUserId = User.Identity.GetUserId();
			ApplicationUser applicationUser = db.ApplicationUsers.Find(currentUserId);
			if (applicationUser != null)
			{
				var currentRoles = this.UserManager.GetRoles(currentUserId);
				var company = db.Companies.Find(applicationUser.CompanyId);
				var companyName = company != null ? company.CompanyName : string.Empty;
				var department = db.Departments.Find(applicationUser.DepartmentId);
				var departmentName = department != null ? department.DepartmentName : string.Empty;
				var result = new ApplicationUserResponse();
				result.Id = applicationUser.Id;
				result.CompanyId = applicationUser.CompanyId;
				result.DepartmentId = applicationUser.DepartmentId;
				result.DisplayName = applicationUser.DisplayName;
				result.ImagePath = applicationUser.ImagePath;
				result.Address = applicationUser.Address;
				result.Age = applicationUser.Age;
				result.AboutMe = applicationUser.AboutMe;
				result.Email = applicationUser.Email;
				result.PhoneNumber = applicationUser.PhoneNumber;
				result.UserName = applicationUser.UserName;
				result.Roles = currentRoles.ToList();
				result.CompanyName = companyName;
				result.DepartmentName = departmentName;
				result.IsSPAdmin = currentRoles.Any(m => m.Equals("SPAdmin"));
				result.IsDirector = currentRoles.Any(m => m.Equals("Director"));
				result.IsStaff = currentRoles.Any(m => m.Equals("Staff"));
				return result;
			}
			return new ApplicationUserResponse();
		}
	}
}