namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "AppointmentDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Bookings", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "Date", c => c.String());
            DropColumn("dbo.Bookings", "AppointmentDate");
        }
    }
}
