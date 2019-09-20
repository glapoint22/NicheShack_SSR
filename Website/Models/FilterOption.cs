using System.Collections.Generic;

namespace Website.Models
{
    public class FilterOption
    {
        public FilterOption()
        {
            ProductFilters = new HashSet<ProductFilter>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int FilterId { get; set; }

        public virtual Filter Filter { get; set; }
        public virtual ICollection<ProductFilter> ProductFilters { get; set; }
    }
}
