namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSomething2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ApplicationUser", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationUser", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
