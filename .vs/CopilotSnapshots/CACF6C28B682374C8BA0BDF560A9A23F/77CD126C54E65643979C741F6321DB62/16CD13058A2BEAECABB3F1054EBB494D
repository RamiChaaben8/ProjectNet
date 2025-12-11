using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToArticleSafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if column exists before adding it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Articles' 
                    AND COLUMN_NAME = 'ImageUrl'
                )
                BEGIN
                    ALTER TABLE [Articles] ADD [ImageUrl] nvarchar(max) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Check if column exists before dropping it
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Articles' 
                    AND COLUMN_NAME = 'ImageUrl'
                )
                BEGIN
                    ALTER TABLE [Articles] DROP COLUMN [ImageUrl];
                END
            ");
        }
    }
}
