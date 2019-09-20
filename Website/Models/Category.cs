using System.Collections.Generic;

namespace Website.Models

{
    public class Category
    {
        public Category()
        {
            Niches = new HashSet<Niche>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<Niche> Niches { get; set; }
    }
}
