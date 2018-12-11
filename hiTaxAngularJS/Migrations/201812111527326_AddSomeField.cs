namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "SH", c => c.String(maxLength: 1));
            AddColumn("dbo.Invoice", "Account", c => c.String(maxLength: 256));
            AddColumn("dbo.Invoice", "SAccount", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "SAccount");
            DropColumn("dbo.Invoice", "Account");
            DropColumn("dbo.Invoice", "SH");
        }
    }
}
