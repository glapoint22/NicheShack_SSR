using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        Task<IEnumerable<ProductReviewDTO>> GetReviews(string productId, string sortBy, int page);
        Task<ProductReviewDTO> GetPositiveReview(string productId);
        Task<ProductReviewDTO> GetNegativeReview(string productId);
    }
}
