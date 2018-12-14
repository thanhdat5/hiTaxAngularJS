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
	[RoutePrefix("api/Invoices")]
	[Authorize]
	public class InvoicesController : ApiControllerBase
	{
		private hiTaxAngularJSDbContext db = new hiTaxAngularJSDbContext();
		private PermissionHelper permissionHelper = new PermissionHelper();

		[Route("GetAll")]
		public HttpResponseMessage Get(HttpRequestMessage request, DateTime fromDate, DateTime toDate)
		{
			return CreateHttpResponse(request, () =>
			{
				var currentUserInfo = permissionHelper.GetUserInfo();
				var result = db.Invoices
				.Where(m => !m.IsDeleted && (m.CreatedDate >= fromDate) && (m.CreatedDate <= toDate))
				.Select(m => new InvoiceResponse
				{
					Id = m.Id,
					Code = m.Code,
					CreatedDate = m.CreatedDate,
					Value = m.Value,
					CompanyId = m.CompanyId ?? 0,
					CompanyName = m.Company != null ? m.Company.CompanyName : string.Empty,
					CustomerId = m.CustomerId,
					CustomerName = m.Customer != null ? m.Customer.CustomerName : string.Empty,
					IsIn = m.IsIn,
					InvoiceType = m.IsIn ? "In" : "Out",
					SH = m.SH,
					InvoiceAccount_Id = m.InvoiceAccount_Id,
					InvoiceSAccount_Id = m.InvoiceSAccount_Id,
				})
				.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
				.OrderByDescending(m => m.CreatedDate)
				.ThenBy(m => m.CompanyName)
				.ThenBy(m => m.Id)
				.ToList();
				foreach (var item in result)
				{
					item.Date = item.CreatedDate.ToShortDateString();

					item.AccountNumber = db.Accounts.Find(item.InvoiceAccount_Id) != null ? db.Accounts.Find(item.InvoiceAccount_Id).AccountNumber : 0;
					item.SAccountNumber = db.Accounts.Find(item.InvoiceSAccount_Id) != null ? db.Accounts.Find(item.InvoiceSAccount_Id).AccountNumber : 0;
				}

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("GetAllDeleted")]
		public HttpResponseMessage GetAllDeleted(HttpRequestMessage request, DateTime fromDate, DateTime toDate)
		{
			return CreateHttpResponse(request, () =>
			{
				var currentUserInfo = permissionHelper.GetUserInfo();
				var result = db.Invoices
				.Where(m => m.IsDeleted == true && (m.CreatedDate >= fromDate) && (m.CreatedDate <= toDate))
				.Select(m => new InvoiceResponse
				{
					Id = m.Id,
					Code = m.Code,
					CreatedDate = m.CreatedDate,
					Value = m.Value,
					CompanyId = m.CompanyId ?? 0,
					CompanyName = m.Company != null ? m.Company.CompanyName : string.Empty,
					CustomerId = m.CustomerId,
					CustomerName = m.Customer != null ? m.Customer.CustomerName : string.Empty,
					IsIn = m.IsIn,
					InvoiceType = m.IsIn ? "In" : "Out",
					SH = m.SH,
					InvoiceAccount_Id = m.InvoiceAccount_Id,
					InvoiceSAccount_Id = m.InvoiceSAccount_Id,
					
				})
				.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
				.OrderByDescending(m => m.CreatedDate)
				.ThenBy(m => m.CompanyName)
				.ThenBy(m => m.Id)
				.ToList();
				foreach (var item in result)
				{
					item.Date = item.CreatedDate.ToShortDateString();
					item.AccountNumber = db.Accounts.Find(item.InvoiceAccount_Id) != null ? db.Accounts.Find(item.InvoiceAccount_Id).AccountNumber : 0;
					item.SAccountNumber = db.Accounts.Find(item.InvoiceSAccount_Id) != null ? db.Accounts.Find(item.InvoiceSAccount_Id).AccountNumber : 0;
				}

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}
		
		[Route("GetDetailsById")]
		public HttpResponseMessage GetDetailsById(HttpRequestMessage request, int id)
		{
			return CreateHttpResponse(request, () =>
			{
				var result = db.InvoiceDetails.Where(m => !m.IsDeleted && m.InvoiceId == id).Select(m => new InvoiceDetailResponse
				{
					Id = m.Id,
					InvoiceId = m.InvoiceId,
					InvoiceCode = m.Invoice != null ? m.Invoice.Code : string.Empty,
					CustomerId = m.Invoice != null ? m.Invoice.CustomerId : 0,
					CustomerName = m.Invoice != null && m.Invoice.Customer != null ? m.Invoice.Customer.CustomerName : string.Empty,
					Value = m.Value,
					DepartmentId = m.DepartmentId,
					DepartmentName = m.Department != null ? m.Department.DepartmentName : string.Empty,
					CategoryId = m.CategoryId,
					CategoryName = m.Category != null ? m.Category.Name : string.Empty,
					TaxValueId = m.TaxValueId,
					ValueTax = m.TaxValue != null ? m.TaxValue.Value : 0,
					InvoiceIsIn = m.Invoice != null ? m.Invoice.IsIn : true,
					InvoiceType = m.Invoice != null ? m.Invoice.IsIn ? "In" : "Out" : string.Empty
				})
				.OrderByDescending(m => m.CategoryName)
				.ThenBy(m => m.Value)
				.ThenBy(m => m.Id).ToList();

				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		[Route("Add")]
		public HttpResponseMessage Post(HttpRequestMessage request, InvoiceRequest requestParam)
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
						var input = new Invoice();
						input.Id = requestParam.Id;
						input.Code = requestParam.Code;
						input.CreatedDate = requestParam.CreatedDate;
						input.Value = requestParam.Value;
						input.CompanyId = requestParam.CompanyId;
						input.CustomerId = requestParam.CustomerId;
						input.IsIn = requestParam.IsIn;

						input.Created = DateTime.Now;
						input.CreatedBy = User.Identity.GetUserId();
						input.Modified = DateTime.Now;
						input.ModifiedBy = User.Identity.GetUserId();

						db.Invoices.Add(input);
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

		[Route("AddV2")]
		public HttpResponseMessage AddV2(HttpRequestMessage request, List<InvoiceInputRequest> requestParam)
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
					var currentUserInfo = permissionHelper.GetUserInfo();
					try
					{
						foreach (var item in requestParam)
						{
							var input = new Invoice();
							input.Id = item.Id;
							input.Code = item.Code;
							input.CreatedDate = item.CreatedDate;
							input.Value = item.Value;
							input.CompanyId = currentUserInfo.CompanyId;
							input.CustomerId = item.CustomerId;
							input.IsIn = item.IsIn;

							input.Created = DateTime.Now;
							input.CreatedBy = User.Identity.GetUserId();
							input.Modified = DateTime.Now;
							input.ModifiedBy = User.Identity.GetUserId();

							db.Invoices.Add(input);
							db.SaveChanges();

							foreach (var inv in item.Details)
							{
								var inputDetail = new InvoiceDetail
								{
									CategoryId = inv.CategoryId,
									DepartmentId = inv.DepartmentId,
									Id = inv.Id,
									InvoiceId = input.Id,
									TaxValueId = inv.TaxValueId,
									IsDeleted = false,
									Value = inv.Value,
									Created = DateTime.Now,
									CreatedBy = User.Identity.GetUserId(),
									Modified = DateTime.Now,
									ModifiedBy = User.Identity.GetUserId()
								};
								db.InvoiceDetails.Add(inputDetail);
							}
							db.SaveChanges();
						}
						response = request.CreateResponse(HttpStatusCode.Created, requestParam);
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
		public HttpResponseMessage Put(HttpRequestMessage request, InvoiceRequest requestParam)
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
					var currentObject = db.Invoices.Find(requestParam.Id);
					if (currentObject != null)
					{
						currentObject.Code = requestParam.Code;
						currentObject.CreatedDate = requestParam.CreatedDate;
						currentObject.Value = requestParam.Value;
						currentObject.CompanyId = requestParam.CompanyId;
						currentObject.CustomerId = requestParam.CustomerId;
						currentObject.IsIn = requestParam.IsIn;

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

		[Route("UpdateAccoundAndSH")]
		[HttpPut]
		public HttpResponseMessage UpdateAccoundAndSH(HttpRequestMessage request, InvoiceAcountRequest requestParam)
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
					var currentObject = db.Invoices.Find(requestParam.Id);
					if (currentObject != null)
					{
						try
						{
							currentObject.SH = requestParam.SH;
							currentObject.InvoiceAccount_Id = requestParam.InvoiceAccount_Id;
							currentObject.InvoiceSAccount_Id = requestParam.InvoiceSAccount_Id;

							currentObject.Modified = DateTime.Now;
							currentObject.ModifiedBy = User.Identity.GetUserId();
							db.SaveChanges();
							response = request.CreateResponse(HttpStatusCode.OK);
						}
						catch (Exception ex)
						{
							response = request.CreateResponse(HttpStatusCode.InternalServerError, ex);
						}
						
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
					var currentObject = db.Invoices.Find(id);
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

		[Route("RevertDelete")]
		[HttpDelete]
		public HttpResponseMessage RevertDelete(HttpRequestMessage request, int id)
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
					var currentObject = db.Invoices.Find(id);
					if (currentObject != null)
					{
						currentObject.IsDeleted = false;
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

		[Route("AccountingReport")]
		[HttpGet]
		public HttpResponseMessage AccountingReport(HttpRequestMessage request, string fromDate, string toDate)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				var result = new List<InvoiceDetailResponse>();

				try
				{
					var invoiceDetails = db.InvoiceDetails.Where(m => !m.IsDeleted);
					var fromDateValue = !string.IsNullOrEmpty(fromDate) ? DateTime.Parse(fromDate) : DateTime.Now;
					var toDateValue = !string.IsNullOrEmpty(toDate) ? DateTime.Parse(toDate) : DateTime.Now;
					foreach (var idetail in invoiceDetails)
					{
						var invoice = db.Invoices.Where(
								m => !m.IsDeleted
								&& m.Id == idetail.InvoiceId
								 && (m.CreatedDate >= fromDateValue)
								&& (m.CreatedDate <= toDateValue)
							  ).FirstOrDefault();
						if (invoice != null)
						{
							var obj = new InvoiceDetailResponse
							{
								InvoiceId = invoice.Id,
								InvoiceCode = invoice.Code,
								CustomerId = invoice.CustomerId,
								CustomerName = db.Customers.Find(invoice.CustomerId).CustomerName,
								CategoryId = idetail.CategoryId,
								CategoryName = idetail.Category.Name,
								InvoiceIsIn = invoice.IsIn,
								InvoiceType = invoice.IsIn ? "In" : "Out",
								Value = idetail.Value,
								ValueTax = idetail.TaxValue.Value
								//InvoiceDetails = _InvoiceDetailRepository.GetListInvoiceDetailsByInvoiceID(idetail.ID)
							};
							result.Add(obj);
						}

					}
					response = request.CreateResponse(HttpStatusCode.OK, result);
				}
				catch (Exception exx)
				{
					response = request.CreateResponse(HttpStatusCode.InternalServerError, exx);
				}
				return response;
			});
		}

		[Route("MonthlyReport")]
		[HttpGet]
		public HttpResponseMessage MonthlyReport(HttpRequestMessage request, string fromDate, string toDate)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				var result = new List<InvoiceDetailResponse>();

				try
				{
					var invoiceDetails = db.InvoiceDetails.Where(m => !m.IsDeleted);
					var fromDateValue = !string.IsNullOrEmpty(fromDate) ? DateTime.Parse(fromDate) : DateTime.Now;
					var toDateValue = !string.IsNullOrEmpty(toDate) ? DateTime.Parse(toDate) : DateTime.Now;
					foreach (var idetail in invoiceDetails)
					{
						var invoice = db.Invoices.Where(
								m => !m.IsDeleted
								&& m.Id == idetail.InvoiceId
								 && (m.CreatedDate >= fromDateValue)
								&& (m.CreatedDate <= toDateValue)
							  ).FirstOrDefault();
						if (invoice != null)
						{
							var obj = new InvoiceDetailResponse
							{
								InvoiceId = invoice.Id,
								InvoiceCode = invoice.Code,
								CustomerId = invoice.CustomerId,
								CustomerName = db.Customers.Find(invoice.CustomerId).CustomerName,
								CategoryId = idetail.CategoryId,
								CategoryName = idetail.Category.Name,
								InvoiceIsIn = invoice.IsIn,
								InvoiceType = invoice.IsIn ? "In" : "Out",
								Value = idetail.Value,
								ValueTax = idetail.TaxValue.Value
								//InvoiceDetails = _InvoiceDetailRepository.GetListInvoiceDetailsByInvoiceID(idetail.ID)
							};
							result.Add(obj);
						}

					}
					response = request.CreateResponse(HttpStatusCode.OK, result);
				}
				catch (Exception exx)
				{
					response = request.CreateResponse(HttpStatusCode.InternalServerError, exx);
				}
				return response;
			});
		}
	}
}
