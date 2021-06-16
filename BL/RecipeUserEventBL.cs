using WebApp2021.DAL;
using WebApp2021.Models;
using System.Threading.Tasks;

namespace WebApp2021.BL
{
    public class RecipeUserEventBL
    {
        private readonly AppDbContext db;

        public RecipeUserEventBL(AppDbContext db) => this.db = db;


        public bool IsFavorite(int userId, int recipeId)
        {
            var e = this.db.RecipeUserEvents.Find(recipeId, userId);
            return e != null ? e.IsFavorite : false;
        }

        public Task<int> RemoveFavorite(int userId, int recipeId)
        {
            var e = this.db.RecipeUserEvents.Find(recipeId, userId);

            if (e == null) 
                return null;

            e.IsFavorite = false;
            this.db.RecipeUserEvents.Update(e);
            
            return this.db.SaveChangesAsync();
        }

        public Task<int> SaveFavoriteRecipe(int userId, int recipeId)
        {
            var e = this.db.RecipeUserEvents.Find(recipeId, userId);

            if (e == null)
                this.db.RecipeUserEvents.Add(new RecipeUserEvent 
                { RecipeId = recipeId, UserId = userId, IsFavorite = true, Views = 0 });
            else
            {
                e.IsFavorite = true;
                this.db.RecipeUserEvents.Update(e);
            }

            return this.db.SaveChangesAsync();
        }

        public Task<int> ViewRecipe(int userId, int recipeId)
        {
            var e = this.db.RecipeUserEvents.Find(recipeId, userId);

            if (e == null)
                this.db.RecipeUserEvents.Add(new RecipeUserEvent
                { RecipeId = recipeId, UserId = userId, IsFavorite = false, Views = 1 });
            else
            {
                e.Views++;
                this.db.RecipeUserEvents.Update(e);
            }

            return this.db.SaveChangesAsync();
        }
    }
}