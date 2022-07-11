namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "State", c => c.String(nullable: false));
            AddColumn("dbo.Shops", "LGA", c => c.String(nullable: false));
            AddColumn("dbo.Shops", "PostCode", c => c.String(nullable: false));
            AlterColumn("dbo.Shops", "Image1", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shops", "Image1", c => c.String(nullable: false));
            DropColumn("dbo.Shops", "PostCode");
            DropColumn("dbo.Shops", "LGA");
            DropColumn("dbo.Shops", "State");
        }
    }
}
