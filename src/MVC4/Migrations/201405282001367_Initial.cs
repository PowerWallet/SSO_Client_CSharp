using System.Data.Entity.Migrations;

namespace FinApps.SSO.MVC4.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                {
                    UserId = c.Int(false, true),
                    Email = c.String(),
                    FirstName = c.String(),
                    LastName = c.String(),
                    PostalCode = c.String(),
                    FinAppsUserToken = c.String(),
                })
                .PrimaryKey(t => t.UserId);
        }

        public override void Down()
        {
            DropTable("dbo.UserProfile");
        }
    }
}