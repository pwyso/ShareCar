namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbackAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        FeedbackID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 160),
                        RatingValue = c.Int(nullable: false),
                        RatingAvg = c.Double(nullable: false),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FeedbackID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "UserID", "dbo.User");
            DropIndex("dbo.Feedbacks", new[] { "UserID" });
            DropTable("dbo.Feedbacks");
        }
    }
}
