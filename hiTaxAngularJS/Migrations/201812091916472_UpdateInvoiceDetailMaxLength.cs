namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInvoiceDetailMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoice", "Code", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoice", "Code", c => c.String(maxLength: 10));
        }
    }
}
