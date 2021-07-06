using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp2021.Migrations
{
    public partial class firstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carbs = table.Column<int>(type: "int", nullable: false),
                    Protein = table.Column<int>(type: "int", nullable: false),
                    Fat = table.Column<int>(type: "int", nullable: false),
                    KosherType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsManager = table.Column<bool>(type: "bit", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreTag",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTag", x => new { x.StoreId, x.TagId });
                    table.ForeignKey(
                        name: "FK_StoreTag_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrepTime = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipe_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredient",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredient", x => new { x.RecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_RecipeIngredient_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeIngredient_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeUserEvent",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeUserEvent", x => new { x.RecipeId, x.UserId });
                    table.ForeignKey(
                        name: "FK_RecipeUserEvent_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeUserEvent_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Ingredient",
                columns: new[] { "Id", "Carbs", "Fat", "KosherType", "Name", "Protein" },
                values: new object[,]
                {
                    { 1, 5, 4, 3, "Egg", 7 },
                    { 12, 4, 20, 2, "Parmesan", 12 },
                    { 11, 4, 20, 0, "Pork", 25 },
                    { 10, 1, 1, 3, "Cucumber", 1 },
                    { 9, 40, 2, 3, "Bread", 1 },
                    { 8, 2, 1, 2, "Milk", 3 },
                    { 13, 20, 5, 3, "Croutons", 1 },
                    { 6, 20, 30, 3, "Mayo", 10 },
                    { 5, 10, 6, 3, "Onion", 4 },
                    { 4, 6, 5, 1, "Chicken", 12 },
                    { 3, 5, 1, 3, "Tomato", 3 },
                    { 2, 26, 10, 3, "Potato", 5 },
                    { 7, 4, 20, 1, "Beef", 25 }
                });

            migrationBuilder.InsertData(
                table: "Recipe",
                columns: new[] { "Id", "ImageURL", "Instructions", "Name", "PrepTime", "UserId", "VideoID" },
                values: new object[] { 5, "https://www.fifteenspatulas.com/wp-content/uploads/2011/10/Caesar-Salad-Fifteen-Spatulas-3.jpg", "Dice the veggies in a bowl, Break some Croutons, add grated Parmesan and olive oil. And now U can eat. its very tasty", "Caesar Salad", 30, 6, "ZwAfROUJIPE" });

            migrationBuilder.InsertData(
                table: "Store",
                columns: new[] { "Id", "City", "Name", "Street" },
                values: new object[,]
                {
                    { 4, "Raanana", "Mega", "Ahuza Street 69" },
                    { 3, "Herzliya", "Tiv Taam", "Ben Gurion Blvd 56" },
                    { 1, "Petah Tikva", "Hazi Hinnam", "Rishon LeTsiyon 1" },
                    { 2, "Kefar Sava", "Mega Sport", "Weizmann 301" }
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, "Food" },
                    { 2, "Sports" },
                    { 3, "Utensils" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "FirstName", "IsManager", "LastName", "Password", "UserName" },
                values: new object[,]
                {
                    { 4, "SagiKerner@gmail.com", "Sagi", false, "Kerner", "Sagik123", "SagiKerner" },
                    { 1, "system@foodies.com", "system", true, "administrator", "admin123", "Admin" },
                    { 2, "DanielBeilin@gmail.com", "Daniel", false, "Beilin", "danielb123", "DanielBeilin" },
                    { 3, "PolaDinitz@gmail.com", "Pola", false, "Dinitz", "polad123", "PolaDinitz" },
                    { 5, "JonathanBarzilai@gmail.com", "Jonathan", false, "Barzilai", "Jonathanb123", "JonathanBarzilai" }
                });

            migrationBuilder.InsertData(
                table: "Recipe",
                columns: new[] { "Id", "ImageURL", "Instructions", "Name", "PrepTime", "UserId", "VideoID" },
                values: new object[,]
                {
                    { 4, "https://media4.s-nbcnews.com/j/newscms/2019_20/1437505/chinatown_chicken_salad_7db57f6a1845004c274840d39ce2b31c.today-inline-large.jpg", "Chop cooked and cooled chicken and place into a large bowl with celery and onions. Mix dressing ingredients (per recipe below) in a bowl. Toss with chicken. Serve on rolls, bread or over a bead of lettuce.", "Chicken Salad", 35, 3, "kUkEBbCOlJU" },
                    { 6, "https://static01.nyt.com/images/2016/06/23/dining/23COOKING-SOY-GRILLED-STEAK1/23COOKING-SOY-GRILLED-STEAK1-articleLarge.jpg", "Take a meat and bbq it", "BBQ Steak", 20, 5, "nsw0Px-Pho8" },
                    { 3, "http://www.thehungrymouse.com/wp-content/uploads/2008/11/dsc07431.jpg", "Break an egg and fry it with bacon", "Omlette with bacon", 15, 2, "7EKpd06AQgk" },
                    { 2, "https://www.everydaymaven.com/wp-content/uploads/2019/04/Arugula-Salad-1.jpg", "Dice the veggies in a bowl", "Salad", 10, 1, "XCmLLzoK3HI" },
                    { 1, "https://st3.depositphotos.com/2208212/19398/i/450/depositphotos_193980236-stock-photo-tortilla-de-patatas-spanish-omelette.jpg", "Break an egg and fry it", "Omlette", 5, 2, "r09Hgeb9-6s" }
                });

            migrationBuilder.InsertData(
                table: "RecipeIngredient",
                columns: new[] { "IngredientId", "RecipeId" },
                values: new object[,]
                {
                    { 3, 5 },
                    { 5, 5 },
                    { 6, 5 },
                    { 10, 5 },
                    { 12, 5 },
                    { 13, 5 }
                });

            migrationBuilder.InsertData(
                table: "RecipeUserEvent",
                columns: new[] { "RecipeId", "UserId", "IsFavorite", "Views" },
                values: new object[,]
                {
                    { 5, 4, true, 0 },
                    { 5, 3, true, 2 },
                    { 5, 2, true, 1 },
                    { 5, 5, false, 1 },
                    { 5, 1, false, 2 }
                });

            migrationBuilder.InsertData(
                table: "StoreTag",
                columns: new[] { "StoreId", "TagId" },
                values: new object[,]
                {
                    { 4, 3 },
                    { 1, 3 },
                    { 3, 2 },
                    { 2, 2 },
                    { 1, 1 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "RecipeIngredient",
                columns: new[] { "IngredientId", "RecipeId" },
                values: new object[,]
                {
                    { 3, 2 },
                    { 5, 4 },
                    { 6, 4 },
                    { 10, 4 },
                    { 11, 3 },
                    { 8, 3 },
                    { 1, 3 },
                    { 4, 4 },
                    { 8, 1 },
                    { 3, 4 },
                    { 5, 6 },
                    { 7, 6 },
                    { 10, 2 },
                    { 6, 2 },
                    { 5, 2 },
                    { 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "RecipeUserEvent",
                columns: new[] { "RecipeId", "UserId", "IsFavorite", "Views" },
                values: new object[,]
                {
                    { 4, 4, true, 1 },
                    { 4, 3, true, 2 },
                    { 4, 2, true, 0 },
                    { 4, 1, true, 2 },
                    { 6, 1, true, 1 },
                    { 6, 2, true, 1 },
                    { 6, 3, true, 2 },
                    { 4, 5, true, 3 },
                    { 3, 2, false, 3 },
                    { 3, 4, true, 3 },
                    { 3, 3, true, 3 },
                    { 6, 4, true, 3 },
                    { 3, 1, true, 0 },
                    { 1, 5, true, 0 },
                    { 1, 4, true, 1 },
                    { 1, 3, true, 0 },
                    { 1, 2, true, 3 },
                    { 1, 1, true, 1 },
                    { 2, 5, false, 3 },
                    { 2, 4, false, 0 },
                    { 2, 3, false, 3 },
                    { 2, 2, true, 3 },
                    { 2, 1, true, 0 },
                    { 3, 5, false, 1 },
                    { 6, 5, false, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_UserId",
                table: "Recipe",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredient_IngredientId",
                table: "RecipeIngredient",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUserEvent_UserId",
                table: "RecipeUserEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTag_TagId",
                table: "StoreTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredient");

            migrationBuilder.DropTable(
                name: "RecipeUserEvent");

            migrationBuilder.DropTable(
                name: "StoreTag");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
