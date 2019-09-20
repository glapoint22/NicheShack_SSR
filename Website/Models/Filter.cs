using System.Collections.Generic;

namespace Website.Models
{
    public class Filter
    {
        public Filter()
        {
            FilterOptions = new HashSet<FilterOption>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FilterOption> FilterOptions { get; set; }
    }
}
