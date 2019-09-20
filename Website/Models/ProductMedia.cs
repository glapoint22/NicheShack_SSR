namespace Website.Models
{
    public class ProductMedia
    {
        public string ProductId { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }

        public virtual Product Product { get; set; }
    }
}
