#pragma warning disable global_usings
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#pragma warning restore global_usings

#nullable disable

namespace PowerTrainer.VideoManagement.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                occured_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                type = table.Column<string>(type: "text", nullable: false),
                data = table.Column<string>(type: "json", nullable: false),
                processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error = table.Column<string>(type: "text", nullable: true),
            },
            constraints: table => table.PrimaryKey("pk_outbox_messages", x => x.id));

        _ = migrationBuilder.CreateTable(
            name: "videos",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                owner_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                is_downloadable = table.Column<bool>(type: "boolean", nullable: false),
                original_file_location = table.Column<string>(type: "text", nullable: false),
                destination_location = table.Column<string>(type: "text", nullable: false),
                stream_file_location = table.Column<string>(type: "text", nullable: true),
                download_file_location = table.Column<string>(type: "text", nullable: true),
                encoding_job_id = table.Column<string>(type: "text", nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                uploaded_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                encoded_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error_message = table.Column<string>(type: "text", nullable: true),
                external_id = table.Column<Guid>(type: "uuid", nullable: false),
            },
            constraints: table => table.PrimaryKey("pk_videos", x => x.id));

        _ = migrationBuilder.CreateIndex(
            name: "ix_videos_encoding_job_id",
            table: "videos",
            column: "encoding_job_id");

        _ = migrationBuilder.CreateIndex(
            name: "ix_videos_external_id",
            table: "videos",
            column: "external_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "outbox_messages");

        _ = migrationBuilder.DropTable(
            name: "videos");
    }
}
