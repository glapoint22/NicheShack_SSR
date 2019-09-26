using System.Collections.Generic;

namespace Website.Models
{
    public class ProductContentType
    {
        public ProductContentType()
        {
            ProductContent = new HashSet<ProductContent>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public virtual ICollection<ProductContent> ProductContent { get; set; }
    }
}
