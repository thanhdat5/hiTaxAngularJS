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

namespace hiTaxAngularJS.Api
{
	public class ApplicationUsersController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		public ApplicationUsersController()
		{
		}
		public ApplicationUsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

		// GET: api/ApplicationUsers
		[Authorize(Roles = "SPAdmin")]
		public IQueryable<ApplicationUser> GetApplicationUsers()
		{
			return db.ApplicationUsers;
		}

		// GET: api/ApplicationUsers/5
		[Authorize(Roles = "SPAdmin")]
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
		[Authorize(Roles = "SPAdmin")]
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
		[Authorize(Roles = "SPAdmin")]
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

		[Authorize(Roles = "SPAdmin")]
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

		[Authorize]
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
					response = request.CreateResponse(HttpStatusCode.OK, result);
				}

				return response;
			});
		}

		[Route("api/ApplicationUsers/GetAll")]
		[Authorize(Roles = "SPAdmin")]
		public HttpResponseMessage GetAll(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.ApplicationUsers.Select(m => new ApplicationUserResponse
				{
					Id = m.Id,
					CompanyId = m.CompanyId,
					DisplayName = m.DisplayName,
					ImagePath = m.ImagePath,
					Address = m.Address,
					Age = m.Age,
					AboutMe = m.AboutMe,
					Email = m.Email,
					PhoneNumber = m.PhoneNumber,
					UserName = m.UserName,
					DepartmentId = m.DepartmentId,
					CompanyName = string.Empty,
					DepartmentName = string.Empty
				}).OrderBy(m => m.UserName).ToList();
				foreach (var item in result)
				{
					item.Roles = this.UserManager.GetRoles(item.Id).ToList();
					item.CompanyName = GetCompanyNameById(item.CompanyId);
					item.DepartmentName = GetDepartmentNameById(item.DepartmentId);
				}
				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		private string GetCompanyNameById(int id)
		{
			string rs;
			var company = db.Companies.FirstOrDefault(m => !m.IsDeleted && m.Id == id);
			rs = company != null ? company.CompanyName : string.Empty;
			return rs;
		}
		private string GetDepartmentNameById(int id)
		{
			string rs;
			var department = db.Departments.FirstOrDefault(m => !m.IsDeleted && m.Id == id);
			rs = department != null ? department.DepartmentName : string.Empty;
			return rs;
		}

		[HttpPost]
		[Authorize]
		[Route("api/ApplicationUsers/UploadImage")]
		public HttpResponseMessage UploadImage(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = "/Content/images/NoImage.gif";
				HttpResponseMessage response = null;
				if (HttpContext.Current.Request.Files.AllKeys.Any())
				{
					var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
					if (httpPostedFile != null)
					{
						var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images"), httpPostedFile.FileName);
						httpPostedFile.SaveAs(fileSavePath);
						result = "/Content/images/" + httpPostedFile.FileName;
					}
				}
				response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Authorize]
		[Route("api/ApplicationUsers/Update")]
		[HttpPut]
		public HttpResponseMessage Update(HttpRequestMessage request, ApplicationUserRequest requestParam)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				if (!ModelState.IsValid)
				{
					request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
				}
				else
				{
					var currentObject = db.ApplicationUsers.Find(requestParam.Id);
					if (currentObject != null)
					{
						if (currentObject.UserName.ToLower() != requestParam.UserName.ToLower())
						{
							var checkUser = db.ApplicationUsers.FirstOrDefault(m => m.Email.ToLower().Equals(requestParam.Email.ToLower()) || m.UserName.ToLower().Equals(requestParam.UserName.ToLower()));
							if (checkUser != null)
							{
								response = request.CreateResponse(HttpStatusCode.Conflict, "Username already exists.");
							}
						}
						if (response == null)
						{
							currentObject.CompanyId = requestParam.CompanyId;
							currentObject.DepartmentId = requestParam.DepartmentId;
							currentObject.DisplayName = requestParam.DisplayName;
							currentObject.Address = requestParam.Address;
							currentObject.Age = requestParam.Age;
							currentObject.AboutMe = requestParam.AboutMe;
							currentObject.ImagePath = requestParam.ImagePath;
							db.SaveChanges();
							response = request.CreateResponse(HttpStatusCode.OK, requestParam);
						}
					}
					else
					{
						response = request.CreateResponse(HttpStatusCode.NotFound, requestParam);
					}
				}
				return response;
			});
		}

		[Authorize]
		[Route("api/ApplicationUsers/Insert")]
		[HttpPost]
		public HttpResponseMessage Insert(HttpRequestMessage request, ApplicationUserRequest requestParam)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				if (!ModelState.IsValid)
				{
					request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
				}
				else
				{
					var checkUser = db.ApplicationUsers.FirstOrDefault(m => m.Email.ToLower().Equals(requestParam.Email.ToLower()) || m.UserName.ToLower().Equals(requestParam.UserName.ToLower()));
					if (checkUser == null)
					{
						var user = new ApplicationUser { UserName = requestParam.Email, Email = requestParam.Email };
						var result = UserManager.Create(user, requestParam.PasswordHash);
						if (result.Succeeded)
						{
							var currentObject = db.ApplicationUsers.Find(user.Id);
							if (currentObject != null)
							{
								currentObject.CompanyId = requestParam.CompanyId;
								currentObject.DepartmentId = requestParam.DepartmentId;
								currentObject.DisplayName = requestParam.DisplayName;
								currentObject.Address = requestParam.Address;
								currentObject.Age = requestParam.Age;
								currentObject.AboutMe = requestParam.AboutMe;
								currentObject.ImagePath = requestParam.ImagePath;
								db.SaveChanges();
								response = request.CreateResponse(HttpStatusCode.OK, requestParam);
							}
							else
							{
								response = request.CreateResponse(HttpStatusCode.NotFound, requestParam);
							}
						}
						else
						{
							response = request.CreateResponse(HttpStatusCode.InternalServerError, requestParam);
						}
					}
					else
					{
						response = request.CreateResponse(HttpStatusCode.Conflict, "Username already exists.");
					}

				}
				return response;
			});
		}

		[Authorize]
		[Route("api/ApplicationUsers/ChangePassword/{id}")]
		[HttpPut]
		public HttpResponseMessage ChangePassword(HttpRequestMessage request, ChangePasswordViewModel requestParam, string id)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				if (!ModelState.IsValid)
				{
					request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
				}
				else
				{
					var userId = id == null || id == "0" ? User.Identity.GetUserId() : id;
					var result = UserManager.ChangePassword(userId, requestParam.OldPassword, requestParam.NewPassword);
					if (result.Succeeded)
					{
						var user = UserManager.FindById(userId);
						if (user != null)
						{
							SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						}
						response = request.CreateResponse(HttpStatusCode.OK, "Password was changed successfully.");
					}
					else
					{
						response = request.CreateResponse(HttpStatusCode.Forbidden, "Old password incorect.");
					}
				}
				return response;
			});
		}

		[Authorize(Roles = "SPAdmin")]
		[Route("api/ApplicationUsers/UpdateRole")]
		[HttpPut]
		public HttpResponseMessage UpdateRole(HttpRequestMessage request, ApplicationUserRoleRequest requestParam)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				if (!ModelState.IsValid)
				{
					request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
				}
				else
				{
					foreach (var role in requestParam.Roles)
					{
						UserManager.AddToRole(requestParam.Id, role);
					}
					response = request.CreateResponse(HttpStatusCode.OK, requestParam);
				}
				return response;
			});
		}
	}
}