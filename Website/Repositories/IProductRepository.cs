using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductDTO>> GetQueriedProducts(QueryParams queryParams);
        Task<IEnumerable<FilterData>> GetProductFilters(QueryParams queryParams, IEnumerable<ProductDTO> products);
    }
}
