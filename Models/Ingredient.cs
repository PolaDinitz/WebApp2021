using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApp2021.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            Recipes = new List<RecipeIngredient>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
        public int Carbs { get; set; }

        [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
        public int Protein { get; set; }

        [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
        public int Fat { get; set; }

        [DisplayName("Calories per 100 gram")]
        public int CaloriesPer100Gram
        {
            get { return Carbs * 4 + Protein * 4 + Fat * 9; }
        }

        [DisplayName("Kosher type")]
        public KosherType KosherType { get; set; }

        [JsonIgnore]
        public virtual List<RecipeIngredient> Recipes { get; set; }
    }
}