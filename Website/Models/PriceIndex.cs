namespace Website.Models
{
    public class PriceIndex
    {
        public int Id { get; set; }
        public string ProductContentId { get; set; }
        public int Index { get; set; }

        public virtual ProductContent ProductContent { get; set; }
    }
}
