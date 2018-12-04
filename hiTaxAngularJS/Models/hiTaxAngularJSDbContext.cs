namespace hiTaxAngularJS.Models
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System.Data.Entity.ModelConfiguration.Conventions;

	public partial class hiTaxAngularJSDbContext : DbContext
	{
		public hiTaxAngularJSDbContext()
			: base("name=hiTaxAngularJSDbContext")
		{
		}
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Company> Companies { get; set; }
		public virtual DbSet<Customer> Customers { get; set; }
		public virtual DbSet<CustomerType> CustomerTypes { get; set; }
		public virtual DbSet<Department> Departments { get; set; }
		public virtual DbSet<Error> Errors { get; set; }
		public virtual DbSet<Invoice> Invoices { get; set; }
		public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<TaxValue> TaxValues { get; set; }
		public virtual DbSet<Unit> Units { get; set; }
		public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

			modelBuilder.Entity<Category>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.Category)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Company>()
				.HasMany(e => e.Departments)
				.WithRequired(e => e.Company)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Customer>()
				.HasMany(e => e.Invoices)
				.WithRequired(e => e.Customer)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Department>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.Department)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Invoice>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.Invoice)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Product>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.Product)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<TaxValue>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.TaxValue)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Unit>()
				.HasMany(e => e.InvoiceDetails)
				.WithRequired(e => e.Unit)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Unit>()
				.HasMany(e => e.Products)
				.WithRequired(e => e.Unit)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<IdentityUserRole>()
				.HasKey(i => new { i.UserId, i.RoleId })
				.ToTable("ApplicationUserRoles");

			modelBuilder.Entity<IdentityUserLogin>()
				.HasKey(i => i.UserId)
				.ToTable("ApplicationUserLogins");

			modelBuilder
				.Entity<IdentityRole>()
				.ToTable("ApplicationRoles");

			modelBuilder
				.Entity<IdentityUserClaim>()
				.HasKey(i => i.UserId)
				.ToTable("ApplicationUserClaims");
		}
		public static hiTaxAngularJSDbContext Create()
		{
			return new hiTaxAngularJSDbContext();
		}
	}
}
