namespace Website.Models
{
    public class ProductFilter
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int FilterOptionId { get; set; }

        public virtual FilterOption FilterOption { get; set; }
        public virtual Product Product { get; set; }
    }
}
