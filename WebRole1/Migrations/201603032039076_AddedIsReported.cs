namespace ShareCar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsReported : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "IsReported", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Feedbacks", "Description", c => c.String(nullable: false, maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Feedbacks", "Description", c => c.String(nullable: false, maxLength: 160));
            DropColumn("dbo.Feedbacks", "IsReported");
        }
    }
}
