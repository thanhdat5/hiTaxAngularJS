﻿using hiTax.Web;
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
	[RoutePrefix("api/Units")]
	[Authorize]
	public class UnitsController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.Units.Where(m => !m.IsDeleted).Select(m => new UnitResponse
				{
					Id = m.Id,
					Name = m.Name,
				}).OrderBy(m => m.Name).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		[Authorize(Roles = "SPAdmin")]
		public HttpResponseMessage Post(HttpRequestMessage request, UnitRequest requestParam)
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
						var input = new Unit();
						input.Id = requestParam.Id;
						input.Name = requestParam.Name;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Units.Add(input);
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
		public HttpResponseMessage Put(HttpRequestMessage request, UnitRequest requestParam)
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
					var currentObject = db.Units.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.Name = requestParam.Name;
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
					var currentObject = db.Units.Find(id);
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
