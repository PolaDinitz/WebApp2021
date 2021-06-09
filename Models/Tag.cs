using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp2021.Models
{
    public class Tag
    {
        public Tag()
        {
            Stores = new List<StoreTag>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public virtual List<StoreTag> Stores { get; set; }
    }        
}
