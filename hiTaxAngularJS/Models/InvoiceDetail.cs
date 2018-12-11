namespace hiTaxAngularJS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceDetail")]
    public partial class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public decimal Value { get; set; }

        public int DepartmentId { get; set; }

        public int CategoryId { get; set; }

        public int TaxValueId { get; set; }

        public Nullable<int> ProductId { get; set; }

        public Nullable<int> UnitId { get; set; }

        public DateTime Created { get; set; }

        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Category Category { get; set; }

        public virtual Department Department { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Product Product { get; set; }

        public virtual TaxValue TaxValue { get; set; }

        public virtual Unit Unit { get; set; }
    }
}
