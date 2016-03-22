namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayRemovedOfferID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Days", "LiftOffer_LiftOfferID", "dbo.LiftOffers");
            DropIndex("dbo.Days", new[] { "LiftOffer_LiftOfferID" });
            RenameColumn(table: "dbo.Days", name: "LiftOffer_LiftOfferID", newName: "LiftOfferID");
            AlterColumn("dbo.Days", "LiftOfferID", c => c.Int(nullable: false));
            CreateIndex("dbo.Days", "LiftOfferID");
            AddForeignKey("dbo.Days", "LiftOfferID", "dbo.LiftOffers", "LiftOfferID", cascadeDelete: true);
            DropColumn("dbo.Days", "OfferID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Days", "OfferID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Days", "LiftOfferID", "dbo.LiftOffers");
            DropIndex("dbo.Days", new[] { "LiftOfferID" });
            AlterColumn("dbo.Days", "LiftOfferID", c => c.Int());
            RenameColumn(table: "dbo.Days", name: "LiftOfferID", newName: "LiftOffer_LiftOfferID");
            CreateIndex("dbo.Days", "LiftOffer_LiftOfferID");
            AddForeignKey("dbo.Days", "LiftOffer_LiftOfferID", "dbo.LiftOffers", "LiftOfferID");
        }
    }
}
