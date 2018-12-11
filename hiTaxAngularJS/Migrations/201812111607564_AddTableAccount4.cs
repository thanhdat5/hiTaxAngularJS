namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAccount4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Invoice", "AccountId");
            DropColumn("dbo.Invoice", "SAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "SAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "AccountId", c => c.Int(nullable: false));
        }
    }
}
