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
	[RoutePrefix("api/InvoiceAcounts")]
	[Authorize]
	public class InvoiceAccountsController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.Accounts.Where(m => !m.IsDeleted).Select(m => new InvoiceAccountResponse
				{
					Id = m.Id,
					AccountNumber = m.AccountNumber,
					IsSymmetric = m.IsSymmetric
				}).OrderBy(m => m.AccountNumber).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("GetInvoiceAccount")]
		public HttpResponseMessage GetInvoiceAccount(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.Accounts.Where(m => !m.IsDeleted).Select(m => new InvoiceAccountResponse//&& !m.IsSymmetric
				{
					Id = m.Id,
					AccountNumber = m.AccountNumber,
					IsSymmetric = m.IsSymmetric
				}).OrderBy(m => m.AccountNumber).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("GetInvoiceSymmetricAccount")]
		public HttpResponseMessage GetInvoiceSymmetricAccount(HttpRequestMessage request)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.Accounts.Where(m => !m.IsDeleted).Select(m => new InvoiceAccountResponse//&& m.IsSymmetric
				{
					Id = m.Id,
					AccountNumber = m.AccountNumber,
					IsSymmetric = m.IsSymmetric
				}).OrderBy(m => m.AccountNumber).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		[Authorize(Roles = "SPAdmin")]
		public HttpResponseMessage Post(HttpRequestMessage request, InvoiceAccountRequest requestParam)
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
						var input = new Account();
						input.Id = requestParam.Id;
						input.AccountNumber = requestParam.AccountNumber;
						input.IsSymmetric = requestParam.IsSymmetric;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Accounts.Add(input);
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
		public HttpResponseMessage Put(HttpRequestMessage request, InvoiceAccountRequest requestParam)
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
					var currentObject = db.Accounts.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.AccountNumber = requestParam.AccountNumber;
						currentObject.IsSymmetric = requestParam.IsSymmetric;
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
					var currentObject = db.Accounts.Find(id);
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
