using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1L, false, "Fiction" },
                    { 2L, false, "Non-Fiction" },
                    { 3L, false, "Science Fiction" },
                    { 4L, false, "Biography" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Available", "CategoryId", "ISBN", "IsDeleted", "PublicationDate", "Quantity", "Title" },
                values: new object[,]
                {
                    { 1L, "Harper Lee", 3L, 1L, "978-0446310789", false, new DateOnly(1960, 7, 11), 5L, "To Kill a Mockingbird" },
                    { 2L, "Jane Austen", 2L, 1L, "978-0141439518", false, new DateOnly(1813, 1, 28), 4L, "Pride and Prejudice" },
                    { 3L, "F. Scott Fitzgerald", 4L, 1L, "978-0743273565", false, new DateOnly(1925, 4, 10), 6L, "The Great Gatsby" },
                    { 4L, "George Orwell", 0L, 1L, "978-0451524935", false, new DateOnly(1949, 6, 8), 3L, "1984" },
                    { 5L, "J.D. Salinger", 1L, 1L, "978-0316769488", true, new DateOnly(1951, 7, 16), 2L, "The Catcher in the Rye" },
                    { 6L, "Yuval Noah Harari", 5L, 2L, "978-0062316097", false, new DateOnly(2014, 9, 1), 7L, "Sapiens: A Brief History of Humankind" },
                    { 7L, "Tara Westover", 3L, 2L, "978-0399590504", false, new DateOnly(2018, 2, 20), 4L, "Educated" },
                    { 8L, "Rebecca Skloot", 2L, 2L, "978-1400052189", false, new DateOnly(2010, 2, 2), 3L, "The Immortal Life of Henrietta Lacks" },
                    { 9L, "Daniel Kahneman", 4L, 2L, "978-0374533557", false, new DateOnly(2011, 10, 25), 5L, "Thinking, Fast and Slow" },
                    { 10L, "Charles Duhigg", 0L, 2L, "978-0812981605", true, new DateOnly(2012, 2, 28), 2L, "The Power of Habit" },
                    { 11L, "Frank Herbert", 5L, 3L, "978-0441013593", false, new DateOnly(1965, 8, 1), 6L, "Dune" },
                    { 12L, "Andy Weir", 3L, 3L, "978-0804139021", false, new DateOnly(2014, 2, 11), 4L, "The Martian" },
                    { 13L, "William Gibson", 2L, 3L, "978-0441569595", false, new DateOnly(1984, 7, 1), 3L, "Neuromancer" },
                    { 14L, "Isaac Asimov", 4L, 3L, "978-0553293357", false, new DateOnly(1951, 5, 1), 5L, "Foundation" },
                    { 15L, "Neal Stephenson", 1L, 3L, "978-0553380958", false, new DateOnly(1992, 6, 1), 2L, "Snow Crash" },
                    { 16L, "Walter Isaacson", 3L, 4L, "978-1451648539", false, new DateOnly(2011, 10, 24), 4L, "Steve Jobs" },
                    { 17L, "Anne Frank", 4L, 4L, "978-0553577129", false, new DateOnly(1947, 6, 25), 5L, "The Diary of a Young Girl" },
                    { 18L, "Michelle Obama", 5L, 4L, "978-1524763138", false, new DateOnly(2018, 11, 13), 6L, "Becoming" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4L);
        }
    }
}
