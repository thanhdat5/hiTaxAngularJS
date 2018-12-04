namespace hiTaxAngularJS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string DepartmentName { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        public int CompanyId { get; set; }

        public DateTime Created { get; set; }

        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Company Company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
