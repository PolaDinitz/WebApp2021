namespace InternetApp2021.Models
{
    public class StoreTag
    {
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}