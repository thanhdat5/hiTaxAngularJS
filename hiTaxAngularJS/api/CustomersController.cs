using hiTax.Web;
using hiTaxAngularJS.Common;
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
	[RoutePrefix("api/Customers")]
	[Authorize]
	public class CustomersController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();
		private PermissionHelper permissionHelper = new PermissionHelper();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var currentUserInfo = permissionHelper.GetUserInfo();
				var result = db.Customers.Where(m => !m.IsDeleted).Select(m => new CustomerResponse
				{
					Id = m.Id,
					CustomerName = m.CustomerName,
					CompanyId = m.CompanyId ?? 0,
					CompanyName = m.Company != null ? m.Company.CompanyName : "",
					CustomerTypeId = m.CustomerTypeId ?? 0,
					CustomerTypeName = m.CustomerType != null ? m.CustomerType.Name : "",
					Address = m.Address,
					PhoneNumber = m.PhoneNumber
				})
				.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
				.OrderBy(m => m.CompanyName)
				.ThenBy(m => m.CustomerName)
				.ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		[Authorize(Roles = "SPAdmin,Director")]
		public HttpResponseMessage Post(HttpRequestMessage request, CustomerRequest requestParam)
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
						var input = new Customer();

						input.Id = requestParam.Id;
						input.CompanyId = requestParam.CompanyId;
						input.CustomerTypeId = requestParam.CustomerTypeId;
						input.CustomerName = requestParam.CustomerName;
						input.Address = requestParam.Address;
						input.PhoneNumber = requestParam.PhoneNumber;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Customers.Add(input);
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
		[Authorize(Roles = "SPAdmin,Director")]
		public HttpResponseMessage Put(HttpRequestMessage request, CustomerRequest requestParam)
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
					var currentObject = db.Customers.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.Id = requestParam.Id;
						currentObject.CompanyId = requestParam.CompanyId;
						currentObject.CustomerTypeId = requestParam.CustomerTypeId;
						currentObject.CustomerName = requestParam.CustomerName;
						currentObject.Address = requestParam.Address;
						currentObject.PhoneNumber = requestParam.PhoneNumber;

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
		[Authorize(Roles = "SPAdmin,Director")]
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
					var currentObject = db.Customers.Find(id);
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
