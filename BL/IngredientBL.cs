using System.Collections.Generic;
using System.Linq;
using WebApp2021.Models;
using WebApp2021.DAL;
using System.Threading.Tasks;

namespace WebApp2021.BL
{
    public class IngredientBL
    {
        private readonly AppDbContext db;

        public IngredientBL(AppDbContext db) => this.db = db;


        public List<Ingredient> GetIngredients()
        {
            return this.db.Ingredients.ToList<Ingredient>();
        }

        public List<string> GetDistinctIngredientsNames()
        {
            return this.db.Ingredients.Select(ing => ing.Name).Distinct().ToList();
        }

        public ValueTask<Ingredient> GetIngredientById(int? id)
        {
            return this.db.Ingredients.FindAsync(id);
        }

        public Task<int> SaveIngredient(Ingredient ingredient)
        {
            this.db.Ingredients.Add(ingredient);
            return this.db.SaveChangesAsync();
        }

        public Task<int> UpdateIngredient(Ingredient ingredient)
        {
            this.db.Ingredients.Update(ingredient);
            return this.db.SaveChangesAsync();
        }

        public async Task<int> DeleteIngredient(int id)
        {
            Ingredient ingredient = await this.GetIngredientById(id);

            this.db.RecipeIngredients.RemoveRange(
                this.db.RecipeIngredients
                .Where(ri => ri.IngredientId == id).ToList());

            foreach (var recipe in ingredient.Recipes.Select(ri => ri.Recipe).ToList())
            {
                recipe.Ingredients.Clear();

                this.db.RecipeUserEvents.RemoveRange(
                    this.db.RecipeUserEvents
                    .Where(re => re.RecipeId == recipe.Id).ToList());
                recipe.Events.Clear();

                this.db.Recipes.Remove(recipe);
            }

            this.db.Ingredients.Remove(ingredient);

            return await this.db.SaveChangesAsync();
        }
    }
}