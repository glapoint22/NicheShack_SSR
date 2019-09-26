using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Get Quick Look Product.....................................................................
        [Route("QuickLookProduct")]
        [HttpGet]
        public async Task<ActionResult> GetQuickLookProduct(string id)
        {
            // Return the product's description and media
            var quickLookProduct = new
            {
                description = await unitOfWork.Products.Get(x => x.Id == id, x => x.Description),
                media = await unitOfWork.Media.GetCollection(x => x.ProductId == id, new ProductMediaDTO())
            };

            return Ok(quickLookProduct);
        }






        // ..................................................................................Get Product Detail.....................................................................
        [Route("ProductDetail")]
        [HttpGet]
        public async Task<ActionResult> GetProductDetail(string id, string sortBy)
        {
            ProductReviewDTO productReviewDTO = new ProductReviewDTO();

            // Get the product based on the id
            ProductDetailDTO product = await unitOfWork.Products.Get(x => x.Id == id, new ProductDetailDTO());
            
            // If the product is found in the database, return the product with other product details
            if (product != null)
            {
                var response = new
                {
                    product,
                    media = await unitOfWork.Media.GetCollection(x => x.ProductId == product.Id, new ProductMediaDTO()),
                    content = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == product.Id, x => new
                    {
                        Type = new {
                            x.ProductContentType.Name,
                            x.ProductContentType.Image
                        },
                        x.Title,
                        PriceIndices = x.PriceIndices.Select(y => y.Index).ToList()
                    }),
                    pricePoints = await unitOfWork.PricePoints.GetCollection(x => x.ProductId == product.Id, x => string.Format(x.Description, x.Price)),
                    reviews = await unitOfWork.ProductReviews.GetReviews(product.Id, sortBy, 1),
                    sortOptions = productReviewDTO.GetSortOptions(),
                    reviewsPerPage = productReviewDTO.GetReviewsPerPage()
                };

                return Ok(response);
            }

            return NotFound();
        }







        // ..................................................................................Get Queried Products.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetQueriedProducts(string query = "", string sort = "", int limit = 24, int categoryId = -1, int nicheId = -1, int page = 1, string filter = "")
        {
            // Set the query params object
            QueryParams queryParams = new QueryParams(query, sort, categoryId, nicheId, filter);

            // Query the products
            IEnumerable<ProductDTO> products = await unitOfWork.Products.GetQueriedProducts(queryParams);

            ProductDTO productDTO = new ProductDTO();

            var response = new
            {
                products = products.Skip((page - 1) * limit).Take(limit).ToList(),
                totalProducts = products.Count(),
                categories = await unitOfWork.Categories.GetQueriedCategories(queryParams, products),
                filters = await unitOfWork.Products.GetProductFilters(queryParams, products),
                numProductsPerPageOptions = productDTO.GetNumProductsPerPageOptions(),
                sortOptions = query != string.Empty ? productDTO.GetSearchSortOptions() : productDTO.GetBrowseSortOptions()
            };

            return Ok(response);
        }
    }
}