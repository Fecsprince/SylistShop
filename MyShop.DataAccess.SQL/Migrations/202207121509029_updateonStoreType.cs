namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateonStoreType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Shops", "StoreTypeId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Shops", "StoreTypeId");
            AddForeignKey("dbo.Shops", "StoreTypeId", "dbo.StoreTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shops", "StoreTypeId", "dbo.StoreTypes");
            DropIndex("dbo.Shops", new[] { "StoreTypeId" });
            DropColumn("dbo.Shops", "StoreTypeId");
            DropTable("dbo.StoreTypes");
        }
    }
}
