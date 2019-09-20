using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        // ..................................................................................Get Categories.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            // Get all categories and their niches
            return Ok(await unitOfWork.Categories.GetCollection(new CategoryDetailDTO()));
        }
    }
}