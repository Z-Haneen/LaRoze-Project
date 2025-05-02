//by fady
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddChildCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No schema changes are needed because ChildCategories is a navigation property
            // and doesn't require a new column in the database.
            // The relationship is already defined by ParentCategoryId.

            // However, if you need to adjust existing data or relationships, you can add logic here.
            // For example, if Categories already exist and you need to establish relationships:
            // migrationBuilder.Sql("UPDATE Categories SET ParentCategoryId = NULL WHERE ParentCategoryId IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No schema changes to revert since ChildCategories is a navigation property.
        }
    }
}