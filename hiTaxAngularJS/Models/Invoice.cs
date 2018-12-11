namespace hiTaxAngularJS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }

        public decimal Value { get; set; }

        public int? CompanyId { get; set; }

        public int CustomerId { get; set; }

        public bool IsIn { get; set; }

        public DateTime Created { get; set; }

        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
		[StringLength(1)]
		public string SH { get; set; }
		public int? InvoiceAccount_Id { get; set; }
		public int? InvoiceSAccount_Id { get; set; }

		public virtual Company Company { get; set; }
		public virtual Account InvoiceAccount { get; set; }
		public virtual Account InvoiceSAccount { get; set; }

		public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
