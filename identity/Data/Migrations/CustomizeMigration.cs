using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace identity.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("CustomizeMigration")]
    public class CustomizeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NEED TO DO THIS BECAUSE THERE IS NOT ANY SUPPORT FOR SPATIAL DATA TYPES IN EF CORE
            // BUG: IF YOU NEED TO ADD SPATIAL DATA TYPE SUPPORT IN EF CORE 10242018
            //migrationBuilder.Sql("ALTER TABLE Application.Countries " +
            //                     "ADD Border geography ");

            //migrationBuilder.Sql("ALTER TABLE Application.Cities " +
            //                     "ADD Location geography ");

            //migrationBuilder.Sql("ALTER TABLE Application.StateProvinces " +
            //                     "ADD Border geography ");
        }
    }
}