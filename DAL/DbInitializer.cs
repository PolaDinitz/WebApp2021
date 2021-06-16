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
                new Ingredient{Id=1, Name="Egg", Carbs=5,Protein=7,Fat=4,KosherType=KosherType.Parve},
                new Ingredient{Id=2, Name="Potato", Carbs=26,Protein=5,Fat=10,KosherType=KosherType.Parve},
                new Ingredient{Id=3, Name="Tomato", Carbs=5,Protein=3,Fat=1,KosherType=KosherType.Parve},
                new Ingredient{Id=4, Name="Chicken", Carbs=6,Protein=12,Fat=5,KosherType=KosherType.Meat},
                new Ingredient{Id=5, Name="Onion", Carbs=10,Protein=4,Fat=6,KosherType=KosherType.Parve},
                new Ingredient{Id=6, Name="Mayo", Carbs=20,Protein=10,Fat=30,KosherType=KosherType.Parve},
                new Ingredient{Id=7, Name="Beef", Carbs=4,Protein=25,Fat=20,KosherType=KosherType.Meat},
                new Ingredient{Id=8, Name="Milk", Carbs=2,Protein=3,Fat=1,KosherType=KosherType.Dairy},
                new Ingredient{Id=9, Name="Bread", Carbs=40,Protein=1,Fat=2,KosherType=KosherType.Parve},
                new Ingredient{Id=10, Name="Cucumber", Carbs=1,Protein=1,Fat=1,KosherType=KosherType.Parve},
                new Ingredient{Id=11, Name="Pork", Carbs=4,Protein=25,Fat=20,KosherType=KosherType.Not_Kosher},
                new Ingredient{Id=12, Name="Parmesan", Carbs=4,Protein=12,Fat=20,KosherType=KosherType.Dairy},
                new Ingredient{Id=13, Name="Croutons", Carbs=20,Protein=1,Fat=5,KosherType=KosherType.Parve},
            };

            modelBuilder.Entity<Ingredient>().HasData(ingredients);
        }

        private static void SeedRecipes(this ModelBuilder modelBuilder)
        {
            var recipes = new List<Recipe>
            {
                new Recipe{Id=1, Name="Omlette", Instructions="Break an egg and fry it", PrepTime=5,
                    ImageURL="https://st3.depositphotos.com/2208212/19398/i/450/depositphotos_193980236-stock-photo-tortilla-de-patatas-spanish-omelette.jpg"
                    ,VideoID="r09Hgeb9-6s",UserId=2,
                },
                new Recipe{Id=2, Name="Salad", Instructions="Dice the veggies in a bowl", PrepTime=10,
                    ImageURL="https://www.everydaymaven.com/wp-content/uploads/2019/04/Arugula-Salad-1.jpg"
                    ,VideoID="XCmLLzoK3HI",UserId=1,
                },
                new Recipe{Id=3, Name="Omlette with bacon", Instructions="Break an egg and fry it with bacon", PrepTime=15,
                    ImageURL="http://www.thehungrymouse.com/wp-content/uploads/2008/11/dsc07431.jpg"
                    ,VideoID="7EKpd06AQgk",UserId=2,
                },
                new Recipe{Id=4, Name="Chicken Salad",
                    Instructions="Chop cooked and cooled chicken and place into a large bowl with celery and onions. Mix dressing ingredients (per recipe below) in a bowl. Toss with chicken. Serve on rolls, bread or over a bead of lettuce.",
                    PrepTime=35,
                    ImageURL="https://media4.s-nbcnews.com/j/newscms/2019_20/1437505/chinatown_chicken_salad_7db57f6a1845004c274840d39ce2b31c.today-inline-large.jpg",
                    VideoID="kUkEBbCOlJU", UserId=3,
                },
                new Recipe{Id=5, Name="Caesar Salad", Instructions="Dice the veggies in a bowl, Break some Croutons, add grated Parmesan and olive oil. And now U can eat. its very tasty", PrepTime=30,
                    ImageURL="https://www.fifteenspatulas.com/wp-content/uploads/2011/10/Caesar-Salad-Fifteen-Spatulas-3.jpg"
                    ,VideoID="ZwAfROUJIPE",UserId=6,
                },
                new Recipe{Id=6, Name="BBQ Steak", Instructions="Take a meat and bbq it", PrepTime=20,
                    ImageURL="https://static01.nyt.com/images/2016/06/23/dining/23COOKING-SOY-GRILLED-STEAK1/23COOKING-SOY-GRILLED-STEAK1-articleLarge.jpg"
                    ,VideoID="nsw0Px-Pho8",UserId=5,
                },
            };

            modelBuilder.Entity<Recipe>().HasData(recipes);

            modelBuilder.SeedRelation<RecipeIngredient>(1, new int[] { 1, 8, });
            modelBuilder.SeedRelation<RecipeIngredient>(2, new int[] { 3, 5, 6, 10, });
            modelBuilder.SeedRelation<RecipeIngredient>(3, new int[] { 1, 8, 11, });
            modelBuilder.SeedRelation<RecipeIngredient>(4, new int[] { 3, 4, 5, 6, 10, });
            modelBuilder.SeedRelation<RecipeIngredient>(5, new int[] { 3, 5, 6, 10, 12, 13, });
            modelBuilder.SeedRelation<RecipeIngredient>(6, new int[] { 5, 7, });

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
                new Store{Id=1, Name = "Hazi Hinnam", Street = "Rishon LeTsiyon 1", City = "Petah Tikva",},
                new Store{Id=2, Name = "Mega Sport", Street = "Weizmann 301", City = "Kefar Sava",},
                new Store{Id=3, Name = "Tiv Taam", Street = "Ben Gurion Blvd 56", City = "Herzliya",},
                new Store{Id=4, Name = "Mega", Street = "Ahuza Street 69", City = "Raanana",},
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