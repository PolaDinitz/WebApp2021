using System;
using System.Collections.Generic;
using System.Linq;
using WebApp2021.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp2021.DAL
{
    public static class DbInitializer
    {
        private static readonly Dictionary<Type, Func<int, int, object>> relationshipsMapper =
            new Dictionary<Type, Func<int, int, object>>
        {
            { typeof(RecipeIngredient), (a, b) => new { RecipeId = a, IngredientId = b } },
            { typeof(RecipeUserEvent), (a, b) => new { RecipeId = a, UserId = b,
                Views = rnd.Next(0, 4), IsFavorite= rnd.Next(2) == 1 } },
            { typeof(StoreTag), (a, b) => new { StoreId = a, TagId = b } },
        };

        private static readonly Random rnd = new Random();

        private static readonly List<User> users = new List<User>
            {
                new User{Id=1, UserName="Admin", FirstName="system", LastName="administrator", Email="system@foodies.com", IsManager=true, Password="admin123"},
                new User{Id=2, UserName="DanielBeilin", FirstName="Daniel",LastName="Beilin",Email="DanielBeilin@gmail.com", IsManager=false, Password="danielb123"},
                new User{Id=3, UserName="PolaDinitz", FirstName="Pola", LastName="Dinitz", Email="PolaDinitz@gmail.com", IsManager=false, Password="polad123"},
                new User{Id=4, UserName="SagiKerner", FirstName="Sagi", LastName="Kerner", Email="SagiKerner@gmail.com", IsManager=false, Password="Sagik123"},
                new User{Id=5, UserName="JonathanBarzilai", FirstName="Jonathan",LastName="Barzilai",Email="JonathanBarzilai@gmail.com",IsManager=false,Password="Jonathanb123"},
            };


        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.SeedUsers();
            modelBuilder.SeedIngredients();
            modelBuilder.SeedRecipes();
            modelBuilder.SeedStores();
        }

        private static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(DbInitializer.users);
        }

        private static void SeedIngredients(this ModelBuilder modelBuilder)
        {
            var ingredients = new List<Ingredient>
            {
                new Ingredient{Id=1, Name="Egg", Carbs=2,Protein=7,Fat=4,KosherType=KosherType.Parve},
                new Ingredient{Id=2, Name="Potato", Carbs=26,Protein=5,Fat=10,KosherType=KosherType.Parve},
                new Ingredient{Id=3, Name="Tomato", Carbs=5,Protein=3,Fat=1,KosherType=KosherType.Parve},
                new Ingredient{Id=4, Name="Chicken", Carbs=6,Protein=20,Fat=2,KosherType=KosherType.Meat},
                new Ingredient{Id=5, Name="Onion", Carbs=10,Protein=4,Fat=6,KosherType=KosherType.Parve},
                new Ingredient{Id=6, Name="Mayo", Carbs=20,Protein=2,Fat=30,KosherType=KosherType.Parve},
                new Ingredient{Id=7, Name="Beef", Carbs=4,Protein=25,Fat=20,KosherType=KosherType.Meat},
                new Ingredient{Id=8, Name="Milk", Carbs=2,Protein=3,Fat=1,KosherType=KosherType.Dairy},
                new Ingredient{Id=9, Name="Bread", Carbs=40,Protein=1,Fat=2,KosherType=KosherType.Parve},
                new Ingredient{Id=10, Name="Cucumber", Carbs=1,Protein=1,Fat=1,KosherType=KosherType.Parve},
                new Ingredient{Id=11, Name="Bacon", Carbs=4,Protein=25,Fat=20,KosherType=KosherType.Not_Kosher},
                new Ingredient{Id=12, Name="Parmesan", Carbs=4,Protein=12,Fat=20,KosherType=KosherType.Dairy},
                new Ingredient{Id=13, Name="Croutons", Carbs=20,Protein=1,Fat=5,KosherType=KosherType.Parve},
                new Ingredient{Id=14, Name="Lemon", Carbs=5,Protein=0,Fat=1,KosherType=KosherType.Parve},
                new Ingredient{Id=15, Name="Flour", Carbs=20,Protein=2,Fat=1,KosherType=KosherType.Parve},
            };

            modelBuilder.Entity<Ingredient>().HasData(ingredients);
        }

        private static void SeedRecipes(this ModelBuilder modelBuilder)
        {
            var recipes = new List<Recipe>
            {
                
                new Recipe{Id=1, Name="Scrumbled egg", Instructions="Break an egg and fry it", PrepTime=5,
                    ImageURL="https://static5.depositphotos.com/1010050/473/i/950/depositphotos_4732778-stock-photo-scrambled-eggs.jpg"
                    ,VideoID="yyi55ZrpJ0E",UserId=5,
                },
                new Recipe{Id=2, Name="Salad", Instructions="Cut the veggies into a bowl", PrepTime=10,
                    ImageURL="https://st.depositphotos.com/1158226/1651/i/950/depositphotos_16519041-stock-photo-closeup-of-healthy-caesar-salad.jpg"
                    ,VideoID="XCmLLzoK3HI",UserId=1,
                },
                new Recipe{Id=3, Name="Omlette with bacon", Instructions="Break an egg and fry it with choped bacon", PrepTime=15,
                    ImageURL="http://www.thehungrymouse.com/wp-content/uploads/2008/11/dsc07431.jpg"
                    ,VideoID="7EKpd06AQgk",UserId=2,
                },
                new Recipe{Id=4, Name="Chicken Salad",
                    Instructions="Chop cooked chicken and place into a bowl with celery and onions. Mix dressing ingredients in a bowl. Toss with chicken. Serve on rolls, bread or over a bead of lettuce.",
                    PrepTime=35,
                    ImageURL="https://st2.depositphotos.com/2716431/6859/i/950/depositphotos_68590239-stock-photo-caesar-salad-with-grilled-chicken.jpg",
                    VideoID="kUkEBbCOlJU", UserId=4,
                },
                new Recipe{Id=5, Name="Chicken Breast", Instructions="Season the chicken and fry it", PrepTime=20,
                    ImageURL="https://st.depositphotos.com/1010050/2532/i/950/depositphotos_25321457-stock-photo-chicken-breast-with-garlic-rub.jpg"
                    ,VideoID="dGePtZflzHQ",UserId=4,
                },
                new Recipe{Id=6, Name="Crispy Chicken", Instructions="Dip the chicken in egg then in bread crumbs with seasoning and fry it", PrepTime=30,
                    ImageURL="https://st.depositphotos.com/1005893/2436/i/950/depositphotos_24366011-stock-photo-fried-chicken.jpg"
                    ,VideoID="r0sFxVgNDLk",UserId=3,
                },
            };

            modelBuilder.Entity<Recipe>().HasData(recipes);

            modelBuilder.SeedRelation<RecipeIngredient>(1, new int[] { 1, 8, });
            modelBuilder.SeedRelation<RecipeIngredient>(2, new int[] { 3, 5, 6, 10, });
            modelBuilder.SeedRelation<RecipeIngredient>(3, new int[] { 1, 8, 11, });
            modelBuilder.SeedRelation<RecipeIngredient>(4, new int[] { 3, 4, 5, 6, 10, });
            modelBuilder.SeedRelation<RecipeIngredient>(5, new int[] { 4, });
            modelBuilder.SeedRelation<RecipeIngredient>(6, new int[] { 4, 1, 15, });

            foreach (var recipe in recipes)
            {
                modelBuilder.SeedRelation<RecipeUserEvent>(recipe.Id,
                    DbInitializer.users.Select(u => u.Id).ToArray());
            }
        }

        private static void SeedStores(this ModelBuilder modelBuilder)
        {
            var tags = new List<Tag>
            {
                new Tag{Id=1, Type = "Food"},
                new Tag{Id=2, Type = "Sports"},
                new Tag{Id=3, Type = "Utensils"},
            };

            modelBuilder.Entity<Tag>().HasData(tags);

            var stores = new List<Store>
            {
                new Store{Id=1, Name = "Shufersal", Street = "Bialik 76", City = "Ramat Gan",},
                new Store{Id=2, Name = "Mega Sport", Street = "Weizmann 301", City = "Kefar Sava",},
                new Store{Id=3, Name = "Tiv Taam", Street = "Ben Gurion Blvd 56", City = "Herzliya",},
                new Store{Id=4, Name = "Keshet Teamim", Street = "Victor Vantura 222", City = "Be'er Sheva",},
            };

            modelBuilder.Entity<Store>().HasData(stores);

            modelBuilder.SeedRelation<StoreTag>(1, new int[] { 1, 3, });
            modelBuilder.SeedRelation<StoreTag>(2, new int[] { 2, });
            modelBuilder.SeedRelation<StoreTag>(3, new int[] { 2, 3, });
            modelBuilder.SeedRelation<StoreTag>(4, new int[] { 3, });
        }

        private static void SeedRelation<Relation>(this ModelBuilder modelBuilder, int primaryId, int[] relatedIds)
            where Relation : class
        {
            foreach (int relatedId in relatedIds)
            {
                modelBuilder.Entity<Relation>()
                    .HasData(relationshipsMapper[typeof(Relation)](primaryId, relatedId));
            }
        }
    }
}