namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatexy : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Services", "Image1", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Image1", c => c.String(nullable: false));
        }
    }
}
