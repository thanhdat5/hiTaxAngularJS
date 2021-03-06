﻿using hiTax.Web;
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
	[RoutePrefix("api/TaxValues")]
	[Authorize]
	public class TaxValuesController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();
		private PermissionHelper permissionHelper = new PermissionHelper();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var currentUserInfo = permissionHelper.GetUserInfo();
				var result = db.TaxValues.Where(m => !m.IsDeleted).Select(m => new TaxValueResponse
				{
					Id = m.Id,
					CompanyId = m.CompanyId ?? 0,
					CompanyName = m.Company != null ? m.Company.CompanyName : "",
					Value = m.Value
				})
				.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
				.OrderBy(m => m.CompanyName)
				.ThenBy(m => m.Value).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		[Authorize(Roles = "SPAdmin")]
		public HttpResponseMessage Post(HttpRequestMessage request, TaxValueRequest requestParam)
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
						var input = new TaxValue();
						input.Id = requestParam.Id;
						input.CompanyId = requestParam.CompanyId;
						input.Value = requestParam.Value;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.TaxValues.Add(input);
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
		[Authorize(Roles = "SPAdmin")]
		public HttpResponseMessage Put(HttpRequestMessage request, TaxValueRequest requestParam)
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
					var currentObject = db.TaxValues.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.CompanyId = requestParam.CompanyId;
						currentObject.Value = requestParam.Value;
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
		[Authorize(Roles = "SPAdmin")]
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
					var currentObject = db.TaxValues.Find(id);
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
