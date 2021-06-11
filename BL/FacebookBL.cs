using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApp2021.DAL;
using WebApp2021.Utils;
using Microsoft.EntityFrameworkCore;

namespace WebApp2021.BL
{
    public enum EventType
    {
        [Description("Liked")]
        Liked,

        [Description("Popular")]
        Viewed
    }

    public class FacebookBL
    {
        private const string accessToken = "EAAWjuUL4rbIBAKtN6zIu0oNjOyRzfxACfZAJcZAZAWUd1eUQbsw1LXOBs4FlFF8ZCwMMj8zLpRJ7B5f7A0W0ZBL6ldO7hnz1BXZAMqSDhZADpfDGYcSGh0wDWcm4jEAcrGfAfBWoDZBSZA2ebiCL2m35BboIA1vSRZAiKHlLp2UMwWKR4WC9HC8igMHHGAgYYGUd4ZD";
        private const string pageID = "104380594636561";
        readonly string facebookAPI = "https://graph.facebook.com/";
        readonly string pageEdgeFeed = "feed";
        readonly string postToPageURL;
        private readonly AppDbContext db;

        public FacebookBL(AppDbContext db)
        {
            this.db = db;
            postToPageURL = $"{facebookAPI}{pageID}/{pageEdgeFeed}";
        }

        public Task<bool> FacebookPostDecider(int recipeId, EventType e)
        {
            float usersCount = this.db.Users.Count();

            if (e == EventType.Viewed)
            {
                float recipeViewsCount = this.db.RecipeUserEvents.Where(rue => rue.RecipeId == recipeId && rue.Views >= 3).Select(rue => rue.Views).Sum();

                return Task.FromResult(recipeViewsCount / usersCount >= 0.8);
            }

            if (e == EventType.Liked)
            {
                float recipeLikesCount = this.db.RecipeUserEvents.Where(rue => rue.RecipeId == recipeId && rue.IsFavorite).Count();

                return Task.FromResult(recipeLikesCount / usersCount >= 0.7);
            }

            return Task.FromResult(false);
        }


        // publish simple text post
        private async void PublishPostToFeed(string postMessage, string imageURL, string videoURL)
        {
            using (var http = new HttpClient())
            {
                var postData = new Dictionary<string, string> {
                    { "access_token", accessToken },
                    { "message", postMessage }
                };

                if (videoURL != null)
                {
                    postData.Add("link", videoURL);
                }
                else if (imageURL != null)
                {
                    postData.Add("link", imageURL);
                }

                var httpResponse = await http.PostAsync(
                    postToPageURL,
                    new FormUrlEncodedContent(postData));

                var httpContent = await httpResponse.Content.ReadAsStringAsync();
            }
        }

        public void PostRecipe(int recipeId, EventType e)
        {
            var recipe = this.db.Recipes.Include("Ingredients").Include("Events").Where(r => r.Id == recipeId).FirstOrDefault();
            var postMessage = $"{e.GetDescription()} Recipe: {recipe.Name}\n";

            postMessage += $"Instructions: {recipe.Instructions}\n";
            postMessage += $"Preperation Time: {recipe.PrepTime} minutes\n";
            postMessage += $"Ingredients: {String.Join(", ", recipe.Ingredients.Select(i => i.Ingredient.Name).ToList())}\n";
            postMessage += $"Published By: {recipe.User.FullName}\n\n";

            float usersCount = this.db.Users.Count();
            float recipeLikesCount = this.db.RecipeUserEvents.Where(rue => rue.RecipeId == recipeId && rue.IsFavorite).Count();

            postMessage += (int)((recipeLikesCount / usersCount) * 100) + "% of the users liked this recipe\n";

            var recipeViews = recipe.Events.Select(rue => rue.Views).Sum();

            postMessage += recipeViews + " users viewed this recipe\n";

            string youtubeVideoURL = null;
            if (recipe.VideoID != null)
            {
                youtubeVideoURL = $"https://www.youtube.com/watch?v={recipe.VideoID}";
                postMessage += youtubeVideoURL + "\n";
            }

            if (recipe.ImageURL != null)
            {
                postMessage += recipe.ImageURL;
            }

            this.PublishPostToFeed(postMessage, recipe.ImageURL, youtubeVideoURL);
        }
    }
}
