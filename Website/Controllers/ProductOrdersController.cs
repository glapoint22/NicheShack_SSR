using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrdersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductOrdersController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Get Orders.....................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetOrders(string filter = "last30", string search = "")
        {
            // Get the customer id from the claims
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // If there are search words
            if (search != string.Empty)
            {
                // This will search for orders with an order id
                var orders = await unitOfWork.ProductOrders.GetOrders(customerId, filter, search);
                if (orders.Count() > 0)
                {
                    // Return an order with the given order id
                    return Ok(new
                    {
                        orders,
                        displayType = "order"
                    });
                }
                else
                {
                    // Search for products in orders
                    return Ok(new
                    {
                        products = await unitOfWork.ProductOrders.GetOrderProducts(customerId, search),
                        displayType = "product"
                    });
                }
            }

            // Return orders based on a time frame
            return Ok(new
            {
                orders = await unitOfWork.ProductOrders.GetOrders(customerId, filter),
                filters = await unitOfWork.ProductOrders.GetOrderFilters(customerId),
                displayType = "order"
            });
        }
    }
}