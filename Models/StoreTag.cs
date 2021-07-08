using Newtonsoft.Json;

namespace WebApp2021.Models
{
    public class StoreTag
    {
        public int StoreId { get; set; }

        [JsonIgnore]
        public virtual Store Store { get; set; }

        public int TagId { get; set; }

        [JsonIgnore]
        public virtual Tag Tag { get; set; }
    }
}
