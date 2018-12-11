namespace hiTaxAngularJS.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	[Table("Account")]
	public partial class Account
	{
		public int Id { get; set; }

		[Required]
		public int AccountNumber { get; set; }

		public bool IsSymmetric { get; set; }

		public DateTime Created { get; set; }

		[StringLength(256)]
		public string CreatedBy { get; set; }

		public DateTime Modified { get; set; }

		[StringLength(256)]
		public string ModifiedBy { get; set; }

		public bool IsDeleted { get; set; }
	}
}
