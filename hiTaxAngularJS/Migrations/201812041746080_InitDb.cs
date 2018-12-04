namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.Int(nullable: false),
                        DisplayName = c.String(),
                        ImagePath = c.String(),
                        Address = c.String(),
                        Age = c.String(),
                        AboutMe = c.String(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserClaims",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ApplicationUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ApplicationUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.ApplicationRoles", t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepartmentId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        TaxValueId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        UnitId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.Unit", t => t.UnitId)
                .ForeignKey("dbo.TaxValue", t => t.TaxValueId)
                .ForeignKey("dbo.Department", t => t.DepartmentId)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .Index(t => t.InvoiceId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CategoryId)
                .Index(t => t.TaxValueId)
                .Index(t => t.ProductId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 256),
                        Address = c.String(maxLength: 500),
                        CompanyId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(maxLength: 256),
                        CustomerTypeId = c.Int(),
                        CompanyId = c.Int(),
                        Address = c.String(maxLength: 500),
                        PhoneNumber = c.String(maxLength: 256),
                        IsDirector = c.Boolean(),
                        Created = c.DateTime(),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.CustomerType", t => t.CustomerTypeId)
                .Index(t => t.CustomerTypeId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.CustomerType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 10),
                        CreatedDate = c.DateTime(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompanyId = c.Int(),
                        CustomerId = c.Int(nullable: false),
                        IsIn = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Customer", t => t.CustomerId)
                .Index(t => t.CompanyId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        ProductName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 500),
                        UnitId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Unit", t => t.UnitId)
                .Index(t => t.CompanyId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.Unit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaxValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        Value = c.Double(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Error",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        StackTrace = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ApplicationRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserRoles", "IdentityRole_Id", "dbo.ApplicationRoles");
            DropForeignKey("dbo.InvoiceDetail", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.InvoiceDetail", "DepartmentId", "dbo.Department");
            DropForeignKey("dbo.InvoiceDetail", "TaxValueId", "dbo.TaxValue");
            DropForeignKey("dbo.TaxValue", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Product", "UnitId", "dbo.Unit");
            DropForeignKey("dbo.InvoiceDetail", "UnitId", "dbo.Unit");
            DropForeignKey("dbo.InvoiceDetail", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Product", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Department", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Invoice", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.InvoiceDetail", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.Invoice", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Customer", "CustomerTypeId", "dbo.CustomerType");
            DropForeignKey("dbo.Customer", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.ApplicationUserRoles", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.ApplicationUserLogins", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.ApplicationUserClaims", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropIndex("dbo.TaxValue", new[] { "CompanyId" });
            DropIndex("dbo.Product", new[] { "UnitId" });
            DropIndex("dbo.Product", new[] { "CompanyId" });
            DropIndex("dbo.Invoice", new[] { "CustomerId" });
            DropIndex("dbo.Invoice", new[] { "CompanyId" });
            DropIndex("dbo.Customer", new[] { "CompanyId" });
            DropIndex("dbo.Customer", new[] { "CustomerTypeId" });
            DropIndex("dbo.Department", new[] { "CompanyId" });
            DropIndex("dbo.InvoiceDetail", new[] { "UnitId" });
            DropIndex("dbo.InvoiceDetail", new[] { "ProductId" });
            DropIndex("dbo.InvoiceDetail", new[] { "TaxValueId" });
            DropIndex("dbo.InvoiceDetail", new[] { "CategoryId" });
            DropIndex("dbo.InvoiceDetail", new[] { "DepartmentId" });
            DropIndex("dbo.InvoiceDetail", new[] { "InvoiceId" });
            DropIndex("dbo.ApplicationUserRoles", new[] { "IdentityRole_Id" });
            DropIndex("dbo.ApplicationUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserClaims", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationRoles");
            DropTable("dbo.Error");
            DropTable("dbo.TaxValue");
            DropTable("dbo.Unit");
            DropTable("dbo.Product");
            DropTable("dbo.Invoice");
            DropTable("dbo.CustomerType");
            DropTable("dbo.Customer");
            DropTable("dbo.Company");
            DropTable("dbo.Department");
            DropTable("dbo.InvoiceDetail");
            DropTable("dbo.Category");
            DropTable("dbo.ApplicationUserRoles");
            DropTable("dbo.ApplicationUserLogins");
            DropTable("dbo.ApplicationUserClaims");
            DropTable("dbo.ApplicationUser");
        }
    }
}
