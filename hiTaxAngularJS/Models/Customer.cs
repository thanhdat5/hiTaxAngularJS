namespace hiTaxAngularJS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Customer")]
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }

        [StringLength(256)]
        public string CustomerName { get; set; }

        public int? CustomerTypeId { get; set; }

        public int? CompanyId { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(256)]
        public string PhoneNumber { get; set; }

        public bool? IsDirector { get; set; }

        public DateTime? Created { get; set; }

        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Company Company { get; set; }

        public virtual CustomerType CustomerType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
