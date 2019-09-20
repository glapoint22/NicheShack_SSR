using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        // Set the context
        private readonly NicheShackContext context;

        public CategoryRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        // ................................................................................Get Queried Categories.....................................................................
        public async Task<IEnumerable<CategoryDTO>> GetQueriedCategories(QueryParams queryParams, IEnumerable<ProductDTO> products)
        {
            List<int> nicheIds = new List<int>();
            List<int> categoryIds = new List<int>();

            // If query params contains category id, add it to the categoryIds list
            if (queryParams.CategoryId != -1)
            {
                categoryIds.Add(queryParams.CategoryId);
            }
            else
            {
                // No category Id is present in the query params, so get a list of nicheIds based on the products and there respective categoryIds
                nicheIds = await GetNicheIds(products);
                categoryIds = await context.Niches
                    .AsNoTracking()
                    .Where(x => nicheIds
                        .Contains(x.Id))
                    .Select(x => x.CategoryId)
                    .Distinct()
                    .ToListAsync();
            }

            // If query params contains niche id, add it to the nicheIds list
            if (queryParams.NicheId != -1)
            {
                nicheIds.Add(queryParams.NicheId);
            }
            else if (nicheIds.Count == 0)
            {
                // No Niche id in the query params so get a list of niche ids based on the products
                nicheIds = await GetNicheIds(products);
            }


            // Return categories & niches based on the category ids and niche ids
            return await context.Categories
                .AsNoTracking()
                .Where(x => categoryIds
                    .Contains(x.Id))
                .Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Niches = x.Niches
                        .Where(y => nicheIds
                            .Contains(y.Id))
                        .Select(y => new NicheDTO
                        {
                            Id = y.Id,
                            Name = y.Name
                        })
                        .ToList()
                })
                .ToListAsync();
        }





        // ................................................................................Get Niche Ids.....................................................................
        private async Task<List<int>> GetNicheIds(IEnumerable<ProductDTO> products)
        {
            // Get niche ids from the products
            return await context.Products
                .AsNoTracking()
                .Where(y => products
                    .Select(x => x.Id)
                    .Contains(y.Id))
                .Select(y => y.NicheId)
                .Distinct()
                .ToListAsync();
        }
    }
}
