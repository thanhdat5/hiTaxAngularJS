namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAccount2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Int(nullable: false),
                        IsSymmetric = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 256),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Invoice", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "SAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "InvoiceAccount_Id", c => c.Int());
            AddColumn("dbo.Invoice", "InvoiceSAccount_Id", c => c.Int());
            AddColumn("dbo.Invoice", "Account_Id", c => c.Int());
            AddColumn("dbo.Invoice", "Account_Id1", c => c.Int());
            CreateIndex("dbo.Invoice", "InvoiceAccount_Id");
            CreateIndex("dbo.Invoice", "InvoiceSAccount_Id");
            CreateIndex("dbo.Invoice", "Account_Id");
            CreateIndex("dbo.Invoice", "Account_Id1");
            AddForeignKey("dbo.Invoice", "InvoiceAccount_Id", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "InvoiceSAccount_Id", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "Account_Id", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "Account_Id1", "dbo.Account", "Id");
            DropColumn("dbo.Invoice", "Account");
            DropColumn("dbo.Invoice", "SAccount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "SAccount", c => c.String(maxLength: 256));
            AddColumn("dbo.Invoice", "Account", c => c.String(maxLength: 256));
            DropForeignKey("dbo.Invoice", "Account_Id1", "dbo.Account");
            DropForeignKey("dbo.Invoice", "Account_Id", "dbo.Account");
            DropForeignKey("dbo.Invoice", "InvoiceSAccount_Id", "dbo.Account");
            DropForeignKey("dbo.Invoice", "InvoiceAccount_Id", "dbo.Account");
            DropIndex("dbo.Invoice", new[] { "Account_Id1" });
            DropIndex("dbo.Invoice", new[] { "Account_Id" });
            DropIndex("dbo.Invoice", new[] { "InvoiceSAccount_Id" });
            DropIndex("dbo.Invoice", new[] { "InvoiceAccount_Id" });
            DropColumn("dbo.Invoice", "Account_Id1");
            DropColumn("dbo.Invoice", "Account_Id");
            DropColumn("dbo.Invoice", "InvoiceSAccount_Id");
            DropColumn("dbo.Invoice", "InvoiceAccount_Id");
            DropColumn("dbo.Invoice", "SAccountId");
            DropColumn("dbo.Invoice", "AccountId");
            DropTable("dbo.Account");
        }
    }
}
