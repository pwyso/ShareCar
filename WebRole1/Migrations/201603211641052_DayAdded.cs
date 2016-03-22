namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        DayID = c.Int(nullable: false, identity: true),
                        DayName = c.String(),
                        Selected = c.Boolean(nullable: false),
                        OfferID = c.Int(nullable: false),
                        LiftOffer_LiftOfferID = c.Int(),
                    })
                .PrimaryKey(t => t.DayID)
                .ForeignKey("dbo.LiftOffers", t => t.LiftOffer_LiftOfferID)
                .Index(t => t.LiftOffer_LiftOfferID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Days", "LiftOffer_LiftOfferID", "dbo.LiftOffers");
            DropIndex("dbo.Days", new[] { "LiftOffer_LiftOfferID" });
            DropTable("dbo.Days");
        }
    }
}
