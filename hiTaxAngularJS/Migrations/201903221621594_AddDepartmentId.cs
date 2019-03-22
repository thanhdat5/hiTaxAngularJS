namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUser", "DepartmentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUser", "DepartmentId");
        }
    }
}
