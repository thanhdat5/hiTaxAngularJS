namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.InvoiceDetail", new[] { "ProductId" });
            DropIndex("dbo.InvoiceDetail", new[] { "UnitId" });
            AlterColumn("dbo.InvoiceDetail", "ProductId", c => c.Int());
            AlterColumn("dbo.InvoiceDetail", "UnitId", c => c.Int());
            CreateIndex("dbo.InvoiceDetail", "ProductId");
            CreateIndex("dbo.InvoiceDetail", "UnitId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InvoiceDetail", new[] { "UnitId" });
            DropIndex("dbo.InvoiceDetail", new[] { "ProductId" });
            AlterColumn("dbo.InvoiceDetail", "UnitId", c => c.Int(nullable: false));
            AlterColumn("dbo.InvoiceDetail", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.InvoiceDetail", "UnitId");
            CreateIndex("dbo.InvoiceDetail", "ProductId");
        }
    }
}
