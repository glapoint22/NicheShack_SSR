using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public interface IProductOrderRepository : IRepository<ProductOrder>
    {
        Task<IEnumerable<ProductOrderDTO>> GetOrders(string customerId, string filter, string searchWords = "");
        Task<IEnumerable<OrderProductQueryResultDTO>> GetOrderProducts(string customerId, string searchWords);
        Task<List<KeyValuePair<string, string>>> GetOrderFilters(string customerId);
    }
}
