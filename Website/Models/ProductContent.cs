using System.Collections.Generic;

namespace Website.Models
{
    public class ProductContent
    {
        public ProductContent()
        {
            PriceIndices = new HashSet<PriceIndex>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public int ProductContentTypeId { get; set; }
        public string Title { get; set; }


        public virtual Product Product { get; set; }
        public virtual ProductContentType ProductContentType { get; set; }
        public virtual ICollection<PriceIndex> PriceIndices { get; set; }
    }
}
