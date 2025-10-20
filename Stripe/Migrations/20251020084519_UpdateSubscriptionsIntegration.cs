using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionsIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "UserSubscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripeProductId",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions",
                column: "StripeSubscriptionId");

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "StripeProductId",
                value: "prod_TFiqRZlgyUG3cJ");

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "StripeProductId",
                value: "prod_TFiqBLnYKGafcO");

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "StripeProductId",
                value: "prod_TFiqRZlgyUG3cJ");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "StripeProductId",
                table: "Subscriptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscriptions",
                table: "UserSubscriptions",
                columns: new[] { "UserId", "SubscriptionId" });
        }
    }
}
