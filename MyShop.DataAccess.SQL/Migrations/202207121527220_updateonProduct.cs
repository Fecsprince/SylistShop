namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateonProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ShopID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Products", "ShopID");
            AddForeignKey("dbo.Products", "ShopID", "dbo.Shops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ShopID", "dbo.Shops");
            DropIndex("dbo.Products", new[] { "ShopID" });
            DropColumn("dbo.Products", "ShopID");
        }
    }
}
