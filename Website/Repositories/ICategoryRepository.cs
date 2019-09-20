using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<CategoryDTO>> GetQueriedCategories(QueryParams queryParams, IEnumerable<ProductDTO> products);
    }
}
