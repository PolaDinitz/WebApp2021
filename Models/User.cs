using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp2021.Models
{
    public class User
    {
        public User()
        {
            Events = new List<RecipeUserEvent>();
            Recipes = new List<Recipe>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Full name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email address")]
        public string Email { get; set; }
        
        [DisplayName("Manager ?")]
        public bool IsManager { get; set; }

        [JsonIgnore]
        [Required]
        [DataType(DataType.Password)] // SHA-256 encrypted
        public string Password { get; set; }

        [JsonIgnore]
        public virtual List<RecipeUserEvent> Events { get; set; }

        [JsonIgnore]
        public virtual List<Recipe> Recipes { get; set; }
    }
}
