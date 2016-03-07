namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RatingAvgToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "RatingAvg", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "RatingAvg", c => c.Double(nullable: false));
        }
    }
}
