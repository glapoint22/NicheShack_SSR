using System.Collections.Generic;

namespace Website.Classes
{
    public struct ProductGroup
    {
        public string Caption;
        public IEnumerable<ProductDTO> Products;
    }
}
