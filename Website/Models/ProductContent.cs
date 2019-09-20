namespace Website.Models
{
    public class ProductContent
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string PriceIndices { get; set; }

        public virtual Product Product { get; set; }
    }
}
