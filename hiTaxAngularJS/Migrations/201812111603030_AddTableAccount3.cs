namespace hiTaxAngularJS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAccount3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoice", "Account_Id", "dbo.Account");
            DropForeignKey("dbo.Invoice", "Account_Id1", "dbo.Account");
            DropIndex("dbo.Invoice", new[] { "Account_Id" });
            DropIndex("dbo.Invoice", new[] { "Account_Id1" });
            DropColumn("dbo.Invoice", "Account_Id");
            DropColumn("dbo.Invoice", "Account_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "Account_Id1", c => c.Int());
            AddColumn("dbo.Invoice", "Account_Id", c => c.Int());
            CreateIndex("dbo.Invoice", "Account_Id1");
            CreateIndex("dbo.Invoice", "Account_Id");
            AddForeignKey("dbo.Invoice", "Account_Id1", "dbo.Account", "Id");
            AddForeignKey("dbo.Invoice", "Account_Id", "dbo.Account", "Id");
        }
    }
}
