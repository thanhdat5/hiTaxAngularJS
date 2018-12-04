namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSomething : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUser", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUser", "Discriminator");
        }
    }
}
