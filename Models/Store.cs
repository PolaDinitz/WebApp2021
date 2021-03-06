using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebApp2021.Utils;


namespace WebApp2021.Models
{
    public class Store
    {
        public Store()
        {
            Tags = new List<StoreTag>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [IsUpdatable]
        [Required]
        public string Name { get; set; }

        [IsUpdatable]
        [Required]
        public string City { get; set; }

        [IsUpdatable]
        [Required]
        public string Street { get; set; }

        public string Location
        {
            get
            {
                return string.Format("{0}, {1}", this.City, this.Street);
            }
        }
        [JsonIgnore]
        public virtual List<StoreTag> Tags { get; set; }
    }
}
