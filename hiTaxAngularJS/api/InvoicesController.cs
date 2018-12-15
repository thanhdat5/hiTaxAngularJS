using ClosedXML.Excel;
using hiTax.Web;
using hiTaxAngularJS.Common;
using hiTaxAngularJS.Models;
using hiTaxAngularJS.Models.Request;
using hiTaxAngularJS.Models.Response;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
				var result = GetAllData(fromDate, toDate);
				HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, result);
				return response;
			});
		}

		private List<InvoiceResponse> GetAllData(DateTime fromDate, DateTime toDate)
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
			return result;
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

		[Route("ExportAccounting")]
		[HttpPost]
		public HttpResponseMessage ExportAccounting(HttpRequestMessage request, DateTime fromDate, DateTime toDate)
		{
			try
			{
				MemoryStream stream;
				using (XLWorkbook wb = new XLWorkbook())
				{
					// Create Table Sheet
					var wsTable = wb.Worksheets.Add("Tabelle1");

					// Init Default Title
					var currentUserInfo = permissionHelper.GetUserInfo();
					var data = db.Invoices
					.Join(db.InvoiceDetails, x => x.Id, y => y.InvoiceId, (x, y) => new { x, y })
					.Where(m => !m.x.IsDeleted && (m.x.CreatedDate >= fromDate) && (m.x.CreatedDate <= toDate))
					.Select(m => new InvoiceExportResponse
					{
						Id = m.x.Id,
						Code = m.x.Code,
						CreatedDate = m.x.CreatedDate,
						Value = m.x.Value,
						CompanyId = m.x.CompanyId ?? 0,
						CompanyName = m.x.Company != null ? m.x.Company.CompanyName : string.Empty,
						CustomerId = m.x.CustomerId,
						CustomerName = m.x.Customer != null ? m.x.Customer.CustomerName : string.Empty,
						IsIn = m.x.IsIn,
						InvoiceType = m.x.IsIn ? "In" : "Out",
						SH = m.x.SH,
						InvoiceAccount_Id = m.x.InvoiceAccount_Id,
						InvoiceSAccount_Id = m.x.InvoiceSAccount_Id,
						DepartmentId = m.y.DepartmentId,
						DepartmentName = m.y.Department != null ? m.y.Department.DepartmentName : string.Empty,
						ValueTax = m.y.TaxValue != null ? m.y.TaxValue.Value : 0
					})
					.Where(m => currentUserInfo.IsSPAdmin || (!currentUserInfo.IsSPAdmin && m.CompanyId == currentUserInfo.CompanyId))
					.OrderByDescending(m => m.CreatedDate)
					.ThenBy(m => m.CompanyName)
					.ThenBy(m => m.Id)
					.ToList();
					foreach (var item in data)
					{
						item.Date = item.CreatedDate.ToShortDateString();

						item.AccountNumber = db.Accounts.Find(item.InvoiceAccount_Id) != null ? db.Accounts.Find(item.InvoiceAccount_Id).AccountNumber : 0;
						item.SAccountNumber = db.Accounts.Find(item.InvoiceSAccount_Id) != null ? db.Accounts.Find(item.InvoiceSAccount_Id).AccountNumber : 0;
					}

					var currRow = 1;

					//draw data row by row
					#region Define columns
					wsTable.Cell(currRow, 1).Value = "No.";
					wsTable.Cell(currRow, 2).Value = "Umsatz (ohne Soll/Haben-Kz)";
					wsTable.Cell(currRow, 3).Value = "Soll/Haben-Kennzeichen";
					wsTable.Cell(currRow, 4).Value = "WKZ Umsatz";
					wsTable.Cell(currRow, 5).Value = "Kurs";
					wsTable.Cell(currRow, 6).Value = "Basis-Umsatz";
					wsTable.Cell(currRow, 7).Value = "WKZ Basis-Umsatz";
					wsTable.Cell(currRow, 8).Value = "Konto";
					wsTable.Cell(currRow, 9).Value = "Gegenkonto (ohne BU-Schlüssel)";
					wsTable.Cell(currRow, 10).Value = "BU-Schlüssel";
					wsTable.Cell(currRow, 11).Value = "Belegdatum";
					wsTable.Cell(currRow, 12).Value = "Belegfeld 1";
					wsTable.Cell(currRow, 13).Value = "Belegfeld 2";
					wsTable.Cell(currRow, 14).Value = "Skonto";
					wsTable.Cell(currRow, 15).Value = "Buchungstext";
					wsTable.Cell(currRow, 16).Value = "Postensperre";
					wsTable.Cell(currRow, 17).Value = "Diverse Adressnummer";
					wsTable.Cell(currRow, 18).Value = "Geschäftspartnerbank";
					wsTable.Cell(currRow, 19).Value = "Sachverhalt";
					wsTable.Cell(currRow, 20).Value = "Zinssperre";
					wsTable.Cell(currRow, 21).Value = "Beleglink";
					wsTable.Cell(currRow, 22).Value = "Beleginfo - Art 1";
					wsTable.Cell(currRow, 23).Value = "Beleginfo - Inhalt 1";
					wsTable.Cell(currRow, 24).Value = "Beleginfo - Art 2";
					wsTable.Cell(currRow, 25).Value = "Beleginfo - Inhalt 2";
					wsTable.Cell(currRow, 26).Value = "Beleginfo - Art 3";
					wsTable.Cell(currRow, 27).Value = "Beleginfo - Inhalt 3";
					wsTable.Cell(currRow, 28).Value = "Beleginfo - Art 4";
					wsTable.Cell(currRow, 29).Value = "Beleginfo - Inhalt 4";
					wsTable.Cell(currRow, 30).Value = "Beleginfo - Art 5";
					wsTable.Cell(currRow, 31).Value = "Beleginfo - Inhalt 5";
					wsTable.Cell(currRow, 32).Value = "Beleginfo - Art 6";
					wsTable.Cell(currRow, 33).Value = "Beleginfo - Inhalt 6";
					wsTable.Cell(currRow, 34).Value = "Beleginfo - Art 7";
					wsTable.Cell(currRow, 35).Value = "Beleginfo - Inhalt 7";
					wsTable.Cell(currRow, 36).Value = "Beleginfo - Art 8";
					wsTable.Cell(currRow, 37).Value = "Beleginfo - Inhalt 8";
					wsTable.Cell(currRow, 38).Value = "KOST1 - Kostenstelle";
					wsTable.Cell(currRow, 39).Value = "KOST1 - Kostenstelle";
					wsTable.Cell(currRow, 40).Value = "KOST-Menge";
					wsTable.Cell(currRow, 41).Value = "EU-Land u. UStID";
					wsTable.Cell(currRow, 42).Value = "EU-Steuersatz";
					wsTable.Cell(currRow, 43).Value = "Abw. Versteuerungsart";
					wsTable.Cell(currRow, 44).Value = "Sachverhalt L+L";
					wsTable.Cell(currRow, 45).Value = "Funktionsergänzung L+L";
					wsTable.Cell(currRow, 46).Value = "BU 49 Hauptfunktionstyp";
					wsTable.Cell(currRow, 47).Value = "BU 49 Hauptfunktionsnummer";
					wsTable.Cell(currRow, 48).Value = "BU 49 Funktionsergänzung";
					wsTable.Cell(currRow, 49).Value = "Zusatzinformation - Art 1";
					wsTable.Cell(currRow, 50).Value = "Zusatzinformation- Inhalt 1";
					wsTable.Cell(currRow, 51).Value = "Zusatzinformation - Art 2";
					wsTable.Cell(currRow, 52).Value = "Zusatzinformation- Inhalt 2";
					wsTable.Cell(currRow, 53).Value = "Zusatzinformation - Art 3";
					wsTable.Cell(currRow, 54).Value = "Zusatzinformation- Inhalt 3";
					wsTable.Cell(currRow, 55).Value = "Zusatzinformation - Art 4";
					wsTable.Cell(currRow, 56).Value = "Zusatzinformation - Art 4";
					wsTable.Cell(currRow, 57).Value = "Zusatzinformation - Art 5";
					wsTable.Cell(currRow, 58).Value = "Zusatzinformation- Inhalt 5";
					wsTable.Cell(currRow, 59).Value = "Zusatzinformation - Art 6";
					wsTable.Cell(currRow, 60).Value = "Zusatzinformation- Inhalt 6";
					wsTable.Cell(currRow, 61).Value = "Zusatzinformation - Art 7";
					wsTable.Cell(currRow, 62).Value = "Zusatzinformation- Inhalt 7";
					wsTable.Cell(currRow, 63).Value = "Zusatzinformation - Art 8";
					wsTable.Cell(currRow, 64).Value = "Zusatzinformation- Inhalt 8";
					wsTable.Cell(currRow, 65).Value = "Zusatzinformation - Art 9";
					wsTable.Cell(currRow, 66).Value = "Zusatzinformation- Inhalt 9";
					wsTable.Cell(currRow, 67).Value = "Zusatzinformation - Art 10";
					wsTable.Cell(currRow, 68).Value = "Zusatzinformation- Inhalt 10";
					wsTable.Cell(currRow, 69).Value = "Zusatzinformation - Art 11";
					wsTable.Cell(currRow, 70).Value = "Zusatzinformation- Inhalt 11";
					wsTable.Cell(currRow, 71).Value = "Zusatzinformation - Art 12";
					wsTable.Cell(currRow, 72).Value = "Zusatzinformation- Inhalt 12";
					wsTable.Cell(currRow, 73).Value = "Zusatzinformation - Art 13";
					wsTable.Cell(currRow, 74).Value = "Zusatzinformation- Inhalt 13";
					wsTable.Cell(currRow, 75).Value = "Zusatzinformation - Art 14";
					wsTable.Cell(currRow, 76).Value = "Zusatzinformation- Inhalt 14";
					wsTable.Cell(currRow, 77).Value = "Zusatzinformation - Art 15";
					wsTable.Cell(currRow, 78).Value = "Zusatzinformation- Inhalt 15";
					wsTable.Cell(currRow, 79).Value = "Zusatzinformation - Art 16";
					wsTable.Cell(currRow, 80).Value = "Zusatzinformation- Inhalt 16";
					wsTable.Cell(currRow, 81).Value = "Zusatzinformation - Art 17";
					wsTable.Cell(currRow, 82).Value = "Zusatzinformation- Inhalt 17";
					wsTable.Cell(currRow, 83).Value = "Zusatzinformation - Art 18";
					wsTable.Cell(currRow, 84).Value = "Zusatzinformation- Inhalt 18";
					wsTable.Cell(currRow, 85).Value = "Zusatzinformation - Art 19";
					wsTable.Cell(currRow, 86).Value = "Zusatzinformation- Inhalt 19";
					wsTable.Cell(currRow, 87).Value = "Zusatzinformation - Art 20";
					wsTable.Cell(currRow, 88).Value = "Zusatzinformation- Inhalt 20";
					wsTable.Cell(currRow, 89).Value = "Stück";
					wsTable.Cell(currRow, 90).Value = "Gewicht";
					wsTable.Cell(currRow, 91).Value = "Zahlweise";
					wsTable.Cell(currRow, 92).Value = "Forderungsart";
					wsTable.Cell(currRow, 93).Value = "Veranlagungsjahr";
					wsTable.Cell(currRow, 94).Value = "Zugeordnete Fälligkeit";
					wsTable.Cell(currRow, 95).Value = "Skontotyp";
					wsTable.Cell(currRow, 96).Value = "Auftragsnummer";
					wsTable.Cell(currRow, 97).Value = "Buchungstyp";
					wsTable.Cell(currRow, 98).Value = "USt-Schlüssel (Anzahlungen)";
					wsTable.Cell(currRow, 99).Value = "EU-Mitgliedstaat (Anzahlungen)";
					wsTable.Cell(currRow, 100).Value = "Sachverhalt L+L (Anzahlungen)";
					wsTable.Cell(currRow, 101).Value = "EU-Steuersatz (Anzahlungen)";
					wsTable.Cell(currRow, 102).Value = "Erlöskonto (Anzahlungen)";
					wsTable.Cell(currRow, 103).Value = "Herkunft-Kz";
					wsTable.Cell(currRow, 104).Value = "Leerfeld";
					wsTable.Cell(currRow, 105).Value = "KOST-Datum";
					wsTable.Cell(currRow, 106).Value = "Mandatsreferenz";
					wsTable.Cell(currRow, 107).Value = "Skontosperre";
					wsTable.Cell(currRow, 108).Value = "Skontosperre";
					wsTable.Cell(currRow, 109).Value = "Beteiligtennummer";
					wsTable.Cell(currRow, 110).Value = "Identifikationsnummer";
					wsTable.Cell(currRow, 111).Value = "Zeichnernummer";
					wsTable.Cell(currRow, 112).Value = "Postensperre bis";
					wsTable.Cell(currRow, 113).Value = "Bezeichnung SoBil-Sachverhalt";
					wsTable.Cell(currRow, 114).Value = "Kennzeichen SoBil-Buchung";
					wsTable.Cell(currRow, 115).Value = "Festschreibung";
					wsTable.Cell(currRow, 116).Value = "Leistungsdatum";
					wsTable.Cell(currRow, 117).Value = "Datum Zord. Steuerperiode";
					#endregion

					//header
					IXLRange header3 = wsTable.Range(currRow, 1, currRow, 117);
					header3.Style.Fill.BackgroundColor = XLColor.LightGray;
					header3.Style.Font.Bold = true;
					currRow++;

					var index = 1;
					foreach (var item in data)
					{
						wsTable.Cell(currRow, 1).Value = index;
						wsTable.Cell(currRow, 2).Value = item.Value / (1 + decimal.Parse(item.ValueTax.ToString()));
						wsTable.Cell(currRow, 3).Value = item.SH;
						wsTable.Cell(currRow, 4).Value = "EUR";
						wsTable.Cell(currRow, 8).Value = item.AccountNumber;
						wsTable.Cell(currRow, 9).Value = item.SAccountNumber;
						wsTable.Cell(currRow, 10).Value = "Ma Thue";
						wsTable.Cell(currRow, 11).Value = item.CreatedDate.ToShortDateString();
						wsTable.Cell(currRow, 12).Value = item.Id;
						wsTable.Cell(currRow, 13).Value = item.Code;
						wsTable.Cell(currRow, 15).Value = "Invoice for " + item.CustomerName;
						wsTable.Cell(currRow, 38).Value = item.DepartmentName;
						index++;
						currRow++;
					}

					IXLRange setBorder = wsTable.Range(1, 1, currRow - 1, 117);
					setBorder.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
					setBorder.Style.Border.RightBorder = XLBorderStyleValues.Thin;
					setBorder.Style.Border.TopBorder = XLBorderStyleValues.Thin;
					setBorder.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

					setBorder.Style.Border.LeftBorderColor = XLColor.Black;
					setBorder.Style.Border.RightBorderColor = XLColor.Black;
					setBorder.Style.Border.TopBorderColor = XLColor.Black;
					setBorder.Style.Border.BottomBorderColor = XLColor.Black;

					//Common lastest
					wsTable.RangeUsed().SetAutoFilter();
					wsTable.Rows().AdjustToContents();
					wsTable.Columns().AdjustToContents();

					stream = GetWorkBookStream(wb);
				}
				HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(stream) };
				response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response.StatusCode = HttpStatusCode.OK;
				return response;
			}
			catch (Exception e)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
			}
		}

		public static MemoryStream GetWorkBookStream(XLWorkbook excelWorkbook)
		{
			MemoryStream fs = new MemoryStream();
			excelWorkbook.SaveAs(fs);
			fs.Position = 0;
			return fs;
		}
	}
}
