using WebApp2021.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApp2021.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeUserEvent> RecipeUserEvents { get; set; }
        public DbSet<StoreTag> StoreTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredient>().HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne<Recipe>(ri => ri.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne<Ingredient>(ri => ri.Ingredient)
                .WithMany(r => r.Recipes)
                .HasForeignKey(ri => ri.IngredientId);


            modelBuilder.Entity<RecipeUserEvent>().HasKey(rv => new { rv.RecipeId, rv.UserId });

            modelBuilder.Entity<RecipeUserEvent>()
                .HasOne<Recipe>(re => re.Recipe)
                .WithMany(r => r.Events)
                .HasForeignKey(re => re.RecipeId);

            modelBuilder.Entity<RecipeUserEvent>()
                .HasOne<User>(re => re.User)
                .WithMany(u => u.Events)
                .HasForeignKey(re => re.UserId);


            modelBuilder.Entity<StoreTag>().HasKey(st => new { st.StoreId, st.TagId });

            modelBuilder.Entity<StoreTag>()
                .HasOne<Store>(st => st.Store)
                .WithMany(s => s.Tags)
                .HasForeignKey(st => st.StoreId);

            modelBuilder.Entity<StoreTag>()
                .HasOne<Tag>(st => st.Tag)
                .WithMany(t => t.Stores)
                .HasForeignKey(st => st.TagId);


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }

            modelBuilder.Seed();

            base.OnModelCreating(modelBuilder);
        }
    }
}