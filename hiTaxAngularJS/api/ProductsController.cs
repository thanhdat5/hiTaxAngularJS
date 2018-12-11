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
	[RoutePrefix("api/Products")]
	[Authorize]
	public class ProductsController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();
		private PermissionHelper permissionHelper = new PermissionHelper();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var currentUserInfo = permissionHelper.GetUserInfo();
				var result = db.Products.Where(m => !m.IsDeleted).Select(m => new ProductResponse
				{
					Id = m.Id,
					ProductName = m.ProductName,
					CompanyId = m.CompanyId ?? 0,
					CompanyName = m.Company != null ? m.Company.CompanyName : "",
					Description = m.Description,
					UnitId = m.UnitId,
					UnitName = m.Unit != null ? m.Unit.Name : "",
				})
				.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
				.OrderBy(m => m.CompanyName)
				.ThenBy(m => m.ProductName)
				.ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		[Authorize(Roles = "SPAdmin,Director")]
		public HttpResponseMessage Post(HttpRequestMessage request, ProductRequest requestParam)
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
						var input = new Product();

						input.Id = requestParam.Id;
						input.CompanyId = requestParam.CompanyId;
						input.UnitId = requestParam.UnitId;
						input.ProductName = requestParam.ProductName;
						input.Description = requestParam.Description;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Products.Add(input);
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
		public HttpResponseMessage Put(HttpRequestMessage request, ProductRequest requestParam)
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
					var currentObject = db.Products.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.Id = requestParam.Id;
						currentObject.CompanyId = requestParam.CompanyId;
						currentObject.UnitId = requestParam.UnitId;
						currentObject.ProductName = requestParam.ProductName;
						currentObject.Description = requestParam.Description;

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
					var currentObject = db.Products.Find(id);
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
