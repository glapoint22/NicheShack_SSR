using System.Collections.Generic;

namespace Website.Classes
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<NicheDTO> Niches { get; set; }
    }
}
