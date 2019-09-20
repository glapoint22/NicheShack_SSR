using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        // Set the context
        private readonly NicheShackContext context;

        public ProductReviewRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        // ..................................................................................Get Reviews.....................................................................
        public async Task<IEnumerable<ProductReviewDTO>> GetReviews(string productId, string sortBy, int page)
        {
            ProductReviewDTO productReviewDTO = new ProductReviewDTO(sortBy);

            return await context.ProductReviews
                .AsNoTracking()
                .SortBy(productReviewDTO)
                .ThenByDescending(x => x.Date)
                .Where(x => x.ProductId == productId)
                .Select(productReviewDTO)
                .Skip((page - 1) * productReviewDTO.ReviewsPerPage)
                .Take(productReviewDTO.ReviewsPerPage)
                .ToListAsync();
        }





        // .............................................................................Get Negative Review................................................................
        public async Task<ProductReviewDTO> GetNegativeReview(string productId)
        {
            return await context.ProductReviews
                .AsNoTracking()
                .OrderBy(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenByDescending(x => x.Date)
                .Where(x => x.ProductId == productId && x.Likes > 0)
                .Select(new ProductReviewDTO())
                .FirstOrDefaultAsync();
        }





        // .............................................................................Get Positive Review................................................................
        public async Task<ProductReviewDTO> GetPositiveReview(string productId)
        {
            return await context.ProductReviews
                .AsNoTracking()
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenByDescending(x => x.Date)
                .Where(x => x.ProductId == productId && x.Likes > 0)
                .Select(new ProductReviewDTO())
                .FirstOrDefaultAsync();
        }
    }
}
