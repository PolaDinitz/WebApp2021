using System;
using System.Collections.Generic;
using System.Linq;
using WebApp2021.Models;
using WebApp2021.DAL;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp2021.Utils;

namespace InternetApp2020.BL
{
    public class RecipeBL
    {
        /**
         * Represents a recipe favorited by users and the number of times it was favorited by them.
         */
        private class RecipeFavorites
        {
            public Recipe recipe { get; set; }
            public int favorites { get; set; }
        }

        /**
         * Represents a recipe a user viewed and the number of times the user viewed it
         */
        private class RecipeViews
        {
            public Recipe recipe { get; set; }
            public int views { get; set; }
        }

        private const int RecommendationPageSize = 6;
        private readonly AppDbContext db;
        private RecipeUserEventBL recipeUserEventBL;

        public RecipeBL(AppDbContext db)
        {
            this.db = db;
            this.recipeUserEventBL = new RecipeUserEventBL(db);
        }


        public ValueTask<Recipe> GetRecipeById(int id)
        {
            return this.db.Recipes.FindAsync(id);
        }

        public Task<int> AddRecipe(Recipe recipe)
        {
            this.db.Recipes.Add(recipe);
            return this.db.SaveChangesAsync();
        }

        public Task<int> UpdateRecipe(Recipe recipe)
        {
            this.db.Recipes.Update(recipe);
            return this.db.SaveChangesAsync();
        }

        public async Task<int> DeleteRecipe(int id)
        {
            Recipe recipe = await this.GetRecipeById(id);

            this.db.RecipeIngredients.RemoveRange(
               this.db.RecipeIngredients
               .Where(ri => ri.RecipeId == id).ToList());

            recipe.Ingredients.Clear();

            this.db.RecipeUserEvents.RemoveRange(
               this.db.RecipeUserEvents
               .Where(re => re.RecipeId == id).ToList());

            recipe.Events.Clear();

            db.Recipes.Remove(recipe);

            return await db.SaveChangesAsync();
        }

        public List<Tuple<Recipe, float>> GetMostPopularRecipes()
        {
            var popularRecipes = (from r in this.db.Recipes.Include("Events")
                                  from e in r.Events
                                  let count = r.Events.Count(e => e.IsFavorite)
                                  select new { Recipe = r, Count = count }
                    )
             .Distinct()
             .OrderByDescending(r => r.Count)
             .Take(RecommendationPageSize)
             .ToList();

            var recipeList = new List<Tuple<Recipe, float>>();

            foreach (var favoriteRec in popularRecipes)
            {
                recipeList.Add(new Tuple<Recipe, float>(favoriteRec.Recipe, favoriteRec.Count));
            }

            return recipeList;
        }

        /* Gets user's recommendation according to different parameters.
         * Each element in the list is a tuple of a recipe and its score.
        */
        public List<Tuple<Recipe, float>> GetUserRecommendation(User user)
        {
            var recipeScores = new List<Tuple<Recipe, float>>();

            foreach (var recipe in this.db.Recipes)
            {
                var recipeScore = this.GetUserRecipeScore(user, recipe);

                recipeScores.Add(new Tuple<Recipe, float>(recipe, recipeScore));
            }

            var descPageRecipeScores = recipeScores
                .OrderByDescending(rs => rs.Item2)
                .Take(RecommendationPageSize)
                .ToList();

            var displayRecipeScores = new List<Tuple<Recipe, float>>();

            foreach (var recipeScore in descPageRecipeScores)
            {
                var timesRecipeFavorited = this.timesRecipeWasFavorited(recipeScore.Item1);

                displayRecipeScores.Add(new Tuple<Recipe, float>(recipeScore.Item1, timesRecipeFavorited));
            }

            return displayRecipeScores;
        }

        /**
         * Calculates the likes ratio of each ingredient that's in a recipe favorited by the given user relative to
         * the total likes of all ingredients.
         * Returns a Tuple of an ingredient and its likes ratio, where ratio of 0 is not liked at all and 1 means that ingredient has all the likes.
         */
        private List<Tuple<Ingredient, float>> GetUserIngredientsLikesRatio(User user)
        {
            // For each ingredient count how many times it's an ingredient of a recipe that is favorited by the user.
            var ingredientsLikes = (
                from ingr in this.db.Ingredients.AsQueryable()
                let likes = (float)this.db.RecipeUserEvents.Count(
                    rue => rue.UserId == user.Id && rue.Recipe.Ingredients.Any(ri => ri.IngredientId == ingr.Id) && rue.IsFavorite == true
                 )
                select new { ingredient = ingr, likes = likes }
            ).ToList();

            float totalLikes = ingredientsLikes.Sum(i => i.likes);

            var ingredientsLikesRatio = new List<Tuple<Ingredient, float>>();

            foreach (var ingredientLiked in ingredientsLikes)
            {
                ingredientsLikesRatio.Add(
                    new Tuple<Ingredient, float>(
                        ingredientLiked.ingredient,
                        totalLikes == 0f ? 0 : (ingredientLiked.likes / totalLikes)
                    )
                );
            }

            return ingredientsLikesRatio;
        }

        /** 
         * Groups each recipe favorited by other users that are NOT the given user and the number of times it was favorited.
         * Returns a list of recipes favorited by other users and the number of times each was favorited.
         */
        private List<RecipeFavorites> GetOtherUsersRecipesFavorites(User user)
        {
            return this.db.RecipeUserEvents
                .AsEnumerable()
                .Where(rue => rue.UserId != user.Id && rue.IsFavorite == true)
                .GroupBy(rue => rue.RecipeId)
                .Select(grp => new RecipeFavorites { recipe = this.db.Recipes.FirstOrDefault(r => r.Id == grp.Key), favorites = grp.Count() })
                .OrderBy(rf => rf.favorites)
                .ToList();
        }

        /**
         * Groups each recipe the user viewed and the number of times the user viewed it.
         * Returns a list of recipes that the users viewed and the number of times each was viewed.
         */
        private List<RecipeViews> GetUserRecipesViews(User user)
        {
            return this.db.RecipeUserEvents
                .AsEnumerable()
                .Where(rue => rue.UserId == user.Id)
                .GroupBy(rue => rue.RecipeId)
                .Select(grp => new RecipeViews { recipe = db.Recipes.FirstOrDefault(r => r.Id == grp.Key), views = grp.Where(rue => rue.RecipeId == grp.Key).FirstOrDefault().Views })
                .OrderBy(rv => rv.views)
                .ToList();
        }

        /**
         * Calculates and returns the total score of the recipe for the given user.
         */
        private float GetUserRecipeScore(User user, Recipe recipe)
        {
            float recipeScore = 0;

            // If the user already liked the recipe keep the recipe score at 0.
            if (!this.recipeUserEventBL.IsFavorite(user.Id, recipe.Id))
            {
                var ingredientsScore = this.GetUserRecipeIngredientsScore(user, recipe);
                var favoritesScore = this.GetUserRecipeFavoritesScore(user, recipe);
                var viewsScore = this.GetUserRecipeViewsScore(user, recipe);

                recipeScore = ingredientsScore + favoritesScore + viewsScore;
            }

            return recipeScore;
        }

        /**
         * Calculates and returns the score for the views of the recipe for the given user.
         */
        private float GetUserRecipeViewsScore(User user, Recipe recipe)
        {
            const float recipeViewsScoreFactor = 0.2f;

            var userRecipesViews = this.GetUserRecipesViews(user);
            int totalTimesUserViewedRecipes = userRecipesViews.Sum(rv => rv.views);

            var viewsScore = userRecipesViews
                .Where(rv => rv.recipe.Id == recipe.Id)
                .Select(rv => rv.views)
                .FirstOrDefault() * recipeViewsScoreFactor;

            if (totalTimesUserViewedRecipes == 0)
            {
                return viewsScore;
            }

            return viewsScore / totalTimesUserViewedRecipes;
        }

        /**
         * Calculates and returns the score for the favorites of the recipe for the given user.
         */
        private float GetUserRecipeFavoritesScore(User user, Recipe recipe)
        {
            const float recipeFavortiesScoreFactor = 0.3f;
            
            var otherUsersRecipesFavorites = this.GetOtherUsersRecipesFavorites(user);
            int totalTimesRecipesWereFavorited = otherUsersRecipesFavorites.Sum(rf => rf.favorites);

            var favoritesScore = otherUsersRecipesFavorites
                .Where(rf => rf.recipe.Id == recipe.Id)
                .Select(rf => rf.favorites)
                .FirstOrDefault() * recipeFavortiesScoreFactor;

            if (totalTimesRecipesWereFavorited == 0)
            {
                return favoritesScore;
            }

            return favoritesScore / totalTimesRecipesWereFavorited;
        }

        /**
         * Calculates and returns the score for the ingredients of the recipe for the given user.
         */
        private float GetUserRecipeIngredientsScore(User user, Recipe recipe)
        {
            const float recipeIngredientsScoreFactor = 0.5f;
            float recipeIngredientsScoreSum = 0;

            var userIngredientsLikesRatio = this.GetUserIngredientsLikesRatio(user);

            foreach (var recipeIngredient in recipe.Ingredients)
            {
                var currentRecipeIngredientScore = userIngredientsLikesRatio
                    .Where(ilr => ilr.Item1.Id == recipeIngredient.Ingredient.Id)
                    .Select(ilr => ilr.Item2)
                    .FirstOrDefault() * (recipeIngredientsScoreFactor / recipe.Ingredients.Count());

                recipeIngredientsScoreSum += currentRecipeIngredientScore;
            }

            return recipeIngredientsScoreSum;
        }

        private int timesRecipeWasFavorited(Recipe recipe)
        {
            return db.RecipeUserEvents.Count(rue => rue.RecipeId == recipe.Id && rue.IsFavorite == true);
        }

        public ICollection<Ingredient> GetIngredients(int recipeID)
        {
            return this.db.Recipes.Find(recipeID)
                .Ingredients.Select(ri => ri.Ingredient).ToList();
        }

        public List<RecipeTotalViews> GetRecipesTotalViews()
        {
            return this.db.RecipeUserEvents
                .AsEnumerable()
                .GroupBy(rue => rue.RecipeId)
                .Select(grp => new RecipeTotalViews
                {
                    recipeName = db.Recipes.FirstOrDefault(r => r.Id == grp.Key).Name,
                    totalViews = grp.Where(rue => rue.RecipeId == grp.Key).Sum(rue => rue.Views) 
                })
                .ToList();
        }

        public List<KosherTypeCount> GetRecipesKosher()
        {
            return this.db.Recipes
                .AsEnumerable()
                .GroupBy(r => r.KosherType)
                .Select(grp => new KosherTypeCount
                {
                    kosherType = grp.Key.GetDescription(),
                    count = grp.Count()
                })
                .ToList();
        }
    }

    public class RecipeTotalViews
    {
        public string recipeName { get; set; }
        public int totalViews { get; set; }
    }

    public class KosherTypeCount
    {
        public string kosherType { get; set; }
        public int count { get; set; }
    }
}