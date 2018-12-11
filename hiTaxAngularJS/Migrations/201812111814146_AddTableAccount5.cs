namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAccount5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoice", "InvoiceAccount_Id", "dbo.Account");
            DropForeignKey("dbo.Invoice", "InvoiceSAccount_Id", "dbo.Account");
            DropIndex("dbo.Invoice", new[] { "InvoiceAccount_Id" });
            DropIndex("dbo.Invoice", new[] { "InvoiceSAccount_Id" });
            AddColumn("dbo.Invoice", "InvoiceAccount_Id1", c => c.Int());
            AddColumn("dbo.Invoice", "InvoiceSAccount_Id1", c => c.Int());
            CreateIndex("dbo.Invoice", "InvoiceAccount_Id1");
            CreateIndex("dbo.Invoice", "InvoiceSAccount_Id1");
            AddForeignKey("dbo.Invoice", "InvoiceAccount_Id1", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "InvoiceSAccount_Id1", "dbo.Account", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoice", "InvoiceSAccount_Id1", "dbo.Account");
            DropForeignKey("dbo.Invoice", "InvoiceAccount_Id1", "dbo.Account");
            DropIndex("dbo.Invoice", new[] { "InvoiceSAccount_Id1" });
            DropIndex("dbo.Invoice", new[] { "InvoiceAccount_Id1" });
            DropColumn("dbo.Invoice", "InvoiceSAccount_Id1");
            DropColumn("dbo.Invoice", "InvoiceAccount_Id1");
            CreateIndex("dbo.Invoice", "InvoiceSAccount_Id");
            CreateIndex("dbo.Invoice", "InvoiceAccount_Id");
            AddForeignKey("dbo.Invoice", "InvoiceSAccount_Id", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "InvoiceAccount_Id", "dbo.Account", "Id");
        }
    }
}
