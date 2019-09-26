using System;
using System.Threading.Tasks;
using Website.Models;

namespace Website.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IProductReviewRepository ProductReviews { get; }
        IListRepository Lists { get; }
        IProductOrderRepository ProductOrders { get; }


        // Generic repositories
        IRepository<ProductMedia> Media { get;  }
        IRepository<ProductContent> ProductContent { get; }
        IRepository<ProductPricePoint> PricePoints { get; }
        IRepository<RefreshToken> RefreshTokens { get; }
        IRepository<ListCollaborator> Collaborators { get; }
        IRepository<Customer> Customers { get; }
        IRepository<ListProduct> ListProducts { get; }


        Task<int> Save();
    }
}
