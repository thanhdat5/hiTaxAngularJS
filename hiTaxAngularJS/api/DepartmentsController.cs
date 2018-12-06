using hiTax.Web;
using hiTaxAngularJS.Models;
using hiTaxAngularJS.Models.Request;
using hiTaxAngularJS.Models.Response;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace hiTaxAngularJS.api
{
	[RoutePrefix("api/Departments")]
	[Authorize]
	public class DepartmentsController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.Departments.Where(m => !m.IsDeleted).Select(m => new DepartmentResponse
				{
					Id = m.Id,
					DepartmentName = m.DepartmentName,
					CompanyId = m.CompanyId,
					CompanyName = m.Company != null ? m.Company.CompanyName : "",
					Address = m.Address
				}).OrderBy(m => m.DepartmentName).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		public HttpResponseMessage Post(HttpRequestMessage request, DepartmentRequest requestParam)
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
					try
					{
						var input = new Department();
						input.Id = requestParam.Id;
						input.CompanyId = requestParam.CompanyId;
						input.DepartmentName = requestParam.DepartmentName;
						input.Address = requestParam.Address;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Departments.Add(input);
						db.SaveChanges();
						response = request.CreateResponse(HttpStatusCode.Created, input);
					}
					catch (Exception ex)
					{
						response = request.CreateResponse(HttpStatusCode.InternalServerError, ex);
					}
				}
				return response;
			});
		}

		[Route("Update")]
		public HttpResponseMessage Put(HttpRequestMessage request, DepartmentRequest requestParam)
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
					var currentObject = db.Departments.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.CompanyId = requestParam.CompanyId;
						currentObject.DepartmentName = requestParam.DepartmentName;
						currentObject.Address = requestParam.Address;
						currentObject.Modified = DateTime.Now;
						currentObject.ModifiedBy = User.Identity.GetUserId();
						db.SaveChanges();
						response = request.CreateResponse(HttpStatusCode.OK);
					}
					else
					{
						response = request.CreateResponse(HttpStatusCode.NotFound);
					}
				}
				return response;
			});
		}

		[Route("Delete")]
		public HttpResponseMessage Delete(HttpRequestMessage request, int id)
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
					var currentObject = db.Departments.Find(id);
					if (currentObject != null)
					{
						currentObject.IsDeleted = true;
						currentObject.Modified = DateTime.Now;
						currentObject.ModifiedBy = User.Identity.GetUserId();
						db.SaveChanges();
						response = request.CreateResponse(HttpStatusCode.OK);
					}
					else
					{
						response = request.CreateResponse(HttpStatusCode.NotFound);
					}
				}
				return response;
			});
		}
	}
}
