using ASP.NET_Razor_Final.models;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Razor_Final.Migrations
{
    /// <inheritdoc />
    public partial class ResetArticlesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Articles");
            //Insert data
            Randomizer.Seed = new Random(8675309);
            var fakerArticle = new Faker<Article>();
            fakerArticle.RuleFor(title => title.Title, f => f.Lorem.Sentence(10, 20));
            fakerArticle.RuleFor(created => created.Created, f => f.Date.Between(new DateTime(2025, 1, 1), new DateTime(2025, 6, 29)));
            fakerArticle.RuleFor(content => content.Content, f => f.Lorem.Paragraphs(13, 20));

            for (int i = 0; i < 150; i++)
            {
                var article = fakerArticle.Generate();
                // Truncate Title to 100 characters to avoid SQL error
                var truncatedTitle = article.Title.Length > 100 ? article.Title.Substring(0, 100) : article.Title;
                migrationBuilder.InsertData(
                table: "articles",
                columns: new[] { "Title", "Created", "Content" },
                values: new object[]{
                    truncatedTitle,
                    article.Created,
                    article.Content
                }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Articles");   
        }
    }
}
