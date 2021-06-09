using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using WebApp2021.Utils;

namespace WebApp2021.Models
{
    public class Recipe
    {
        public Recipe()
        {
            Ingredients = new List<RecipeIngredient>();
            Events = new List<RecipeUserEvent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [IsUpdatable]
        [Required]
        public string Name { get; set; }

        [IsUpdatable]
        [Required]
        public string Instructions { get; set; }

        [IsUpdatable]
        [Required]
        [DisplayName("Time to prepare")]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be at least 1")]
        public int PrepTime { get; set; }

        [IsUpdatable]
        [DataType(DataType.Url)]
        [DisplayName("Image")]
        public string ImageURL { get; set; }

        [IsUpdatable]
        [DataType(DataType.Text)]
        [DisplayName("Video")]
        public string VideoID { get; set; }

        public KosherType KosherType
        {
            get
            {
                /* Get recipe's kosher type using bitwise and operation(&).
                 * 
                 * A Parve ingredient is neutral, so it has a mask value of 3 --> 2 LSB are 11.
                 * A Not Kosher ingredient makes the recipe Not Kosher any way,
                 *  so it has value of 0 --> 2 LSB are 00.
                 *  
                 * Meat and Dairy ingredients must be seperated otherwise the recipe is not kosher,
                 *  so Meat's value is 1 --> 2 LSB are 01, and Dairy's value is 2 --> 2 LSB are 10,
                 *  then if we do (01 & 10) bitwise operation, we get 00 means Not Kosher as excepted.
                 * */

                return (KosherType)Ingredients.Select(x => (int)x.Ingredient.KosherType)
                        .Aggregate((int)KosherType.Parve, (x, y) => x & y);
            }
        }

        [DisplayName("Creator")]
        public virtual User User { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public virtual List<RecipeIngredient> Ingredients { get; set; }

        [JsonIgnore]
        public virtual List<RecipeUserEvent> Events { get; set; }
    }
}
