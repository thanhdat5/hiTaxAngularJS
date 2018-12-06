using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using hiTax.Web;
using hiTaxAngularJS.Models;
using hiTaxAngularJS.Models.Response;
using Microsoft.AspNet.Identity;

namespace hiTaxAngularJS.Api
{
	public class ApplicationUsersController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();

		// GET: api/ApplicationUsers
		public IQueryable<ApplicationUser> GetApplicationUsers()
		{
			return db.ApplicationUsers;
		}

		[Route("api/ApplicationUsers/GetProfile")]
		public HttpResponseMessage GetProfile(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				var currentUserId = User.Identity.GetUserId();
				ApplicationUser applicationUser = db.ApplicationUsers.Find(currentUserId);
				if (applicationUser == null)
				{
					response = request.CreateResponse(HttpStatusCode.NotFound);
				}
				else
				{
					var company = db.Companies.Find(applicationUser.CompanyId);
					var companyName = company != null ? company.CompanyName : string.Empty;
					var result = new ApplicationUserResponse();
					result.Id = applicationUser.Id;
					result.CompanyId = applicationUser.CompanyId;
					result.DisplayName = applicationUser.DisplayName;
					result.ImagePath = applicationUser.ImagePath;
					result.Address = applicationUser.Address;
					result.Age = applicationUser.Age;
					result.AboutMe = applicationUser.AboutMe;
					result.Email = applicationUser.Email;
					result.PhoneNumber = applicationUser.PhoneNumber;
					result.UserName = applicationUser.UserName;
					result.PasswordHash = applicationUser.PasswordHash;
					result.CompanyName = companyName;
					response = request.CreateResponse(HttpStatusCode.OK, result);
				}

				return response;
			});
		}

		// GET: api/ApplicationUsers/5
		[ResponseType(typeof(ApplicationUser))]
		public IHttpActionResult GetApplicationUser(string id)
		{
			ApplicationUser applicationUser = db.ApplicationUsers.Find(id);
			if (applicationUser == null)
			{
				return NotFound();
			}

			return Ok(applicationUser);
		}

		// PUT: api/ApplicationUsers/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutApplicationUser(string id, ApplicationUser applicationUser)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != applicationUser.Id)
			{
				return BadRequest();
			}

			db.Entry(applicationUser).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ApplicationUserExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/ApplicationUsers
		[ResponseType(typeof(ApplicationUser))]
		public IHttpActionResult PostApplicationUser(ApplicationUser applicationUser)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.ApplicationUsers.Add(applicationUser);

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateException)
			{
				if (ApplicationUserExists(applicationUser.Id))
				{
					return Conflict();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("DefaultApi", new { id = applicationUser.Id }, applicationUser);
		}

		// DELETE: api/ApplicationUsers/5
		[ResponseType(typeof(ApplicationUser))]
		public IHttpActionResult DeleteApplicationUser(string id)
		{
			ApplicationUser applicationUser = db.ApplicationUsers.Find(id);
			if (applicationUser == null)
			{
				return NotFound();
			}

			db.ApplicationUsers.Remove(applicationUser);
			db.SaveChanges();

			return Ok(applicationUser);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool ApplicationUserExists(string id)
		{
			return db.ApplicationUsers.Count(e => e.Id == id) > 0;
		}
	}
}