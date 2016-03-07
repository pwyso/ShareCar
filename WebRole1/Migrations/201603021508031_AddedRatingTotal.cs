namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRatingTotal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "LeftBy", c => c.String());
            AddColumn("dbo.User", "RatingAvg", c => c.Double(nullable: false));
            DropColumn("dbo.Feedbacks", "RatingAvg");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Feedbacks", "RatingAvg", c => c.Double(nullable: false));
            DropColumn("dbo.User", "RatingAvg");
            DropColumn("dbo.Feedbacks", "LeftBy");
        }
    }
}
