namespace WebApp2021.Models
{
    public class RecipeUserEvent
    {
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int Views { get; set; }
        public bool IsFavorite { get; set; }
    }
}
