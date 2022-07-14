namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasketItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BasketId = c.String(maxLength: 128),
                        ProductId = c.String(),
                        Quantity = c.Int(nullable: false),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Baskets", t => t.BasketId)
                .Index(t => t.BasketId);
            
            CreateTable(
                "dbo.Baskets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ServiceID = c.String(maxLength: 128),
                        UserID = c.String(nullable: false),
                        AppointmentDate = c.DateTime(nullable: false),
                        AppointmentTime = c.String(nullable: false),
                        Address = c.String(maxLength: 100),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.ServiceID)
                .Index(t => t.ServiceID);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 50),
                        ShopID = c.String(maxLength: 128),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Image1 = c.String(),
                        Image2 = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shops", t => t.ShopID)
                .Index(t => t.ShopID);
            
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StoreTypeId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        Location = c.String(nullable: false, maxLength: 100),
                        State = c.String(nullable: false),
                        LGA = c.String(nullable: false),
                        PostCode = c.String(nullable: false),
                        Contact1 = c.String(nullable: false, maxLength: 11),
                        Contact2 = c.String(nullable: false, maxLength: 11),
                        Image1 = c.String(),
                        Image2 = c.String(),
                        UserID = c.String(nullable: false),
                        Days = c.String(nullable: false),
                        OpeningHour = c.String(nullable: false),
                        ClosingHour = c.String(nullable: false),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoreTypes", t => t.StoreTypeId, cascadeDelete: true)
                .Index(t => t.StoreTypeId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        CategoryID = c.String(maxLength: 128),
                        ShopID = c.String(maxLength: 128),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 100),
                        Image1 = c.String(),
                        Image2 = c.String(),
                        UserID = c.String(nullable: false),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategories", t => t.CategoryID)
                .ForeignKey("dbo.Shops", t => t.ShopID)
                .Index(t => t.CategoryID)
                .Index(t => t.ShopID);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 100),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserID = c.Int(nullable: false),
                        ProductID = c.String(maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zipcode = c.String(),
                        OrderStatus = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProductId = c.String(),
                        OrderId = c.String(maxLength: 128),
                        ProductName = c.String(),
                        Image1 = c.String(),
                        Image2 = c.String(),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.StoreTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zipcode = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "ServiceID", "dbo.Services");
            DropForeignKey("dbo.Services", "ShopID", "dbo.Shops");
            DropForeignKey("dbo.Shops", "StoreTypeId", "dbo.StoreTypes");
            DropForeignKey("dbo.Products", "ShopID", "dbo.Shops");
            DropForeignKey("dbo.Orders", "ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.ProductCategories");
            DropForeignKey("dbo.BasketItems", "BasketId", "dbo.Baskets");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.Orders", new[] { "ProductID" });
            DropIndex("dbo.Products", new[] { "ShopID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.Shops", new[] { "StoreTypeId" });
            DropIndex("dbo.Services", new[] { "ShopID" });
            DropIndex("dbo.Bookings", new[] { "ServiceID" });
            DropIndex("dbo.BasketItems", new[] { "BasketId" });
            DropTable("dbo.Customers");
            DropTable("dbo.StoreTypes");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Products");
            DropTable("dbo.Shops");
            DropTable("dbo.Services");
            DropTable("dbo.Bookings");
            DropTable("dbo.Baskets");
            DropTable("dbo.BasketItems");
        }
    }
}
