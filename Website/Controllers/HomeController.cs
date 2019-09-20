using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Get.....................................................................
        public async Task<ActionResult> Get()
        {
            List<ProductGroup> productGroups = new List<ProductGroup>();

            // Featured products
            ProductGroup featuredProducts = new ProductGroup
            {
                Caption = "Check out our featured products",
                Products = await unitOfWork.Products.GetCollection(x => x.Featured, new ProductDTO())
            };

            // Add featured products to the list of product groups
            productGroups.Add(featuredProducts);


            return Ok(new
            {
                productGroups,
            });
        }
    }
}