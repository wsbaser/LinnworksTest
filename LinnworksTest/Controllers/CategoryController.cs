using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinnworksTest.DataAccess;
using LinnworksTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinnworksTest.Controllers
{
    [Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CategoryController : Controller
	{
        private readonly IGenericRepository<DataAccess.Category> categoryRepository;

        public CategoryController(IGenericRepository<DataAccess.Category> categoryRepository)        
        {
            this.categoryRepository = categoryRepository;
        }

		[HttpGet("[action]")]
		public async Task<IEnumerable<CategoryWithStock>> Index()
		{			
            var categories = await categoryRepository.GetAllAsync();

			return categories.Select(x => new CategoryWithStock
			{
				CategoryId = x.Id,
				CategoryName = x.CategoryName,
				Stock = 0
			});
		}

		[HttpGet]
		[Route("[action]/{id}")]
		public async Task<ActionResult> Details(string id)
		{
			if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid categoryId))
				return new BadRequestResult();


            var category = await categoryRepository.GetByIdAsync(categoryId);
			if (category == null)
				return new NotFoundResult();

			return new OkObjectResult(new Models.Category { CategoryId = category.Id, CategoryName = category.CategoryName });
		}

		[HttpPost("[action]")]
		public async Task<Models.Category> Create([FromBody]Models.Category category)
		{
			var created = await categoryRepository.CreateAsync(new DataAccess.Category() { CategoryName = category.CategoryName });

            return new Models.Category
            {
                CategoryId = created.Id,
                CategoryName = created.CategoryName
            };
		}

		[HttpPut("[action]")]
		public async Task Edit([FromBody]Models.Category category)
		{			
            await categoryRepository.UpdateAsync(category.CategoryId, new DataAccess.Category { Id = category.CategoryId, CategoryName = category.CategoryName });
		}

		[HttpDelete("[action]/{id}")]
		public async Task<ActionResult> Delete(string id)
		{
			if (!Guid.TryParse(id, out Guid categoryId))
				return new BadRequestResult();

            await categoryRepository.DeleteAsync(categoryId);

			return new OkResult();
		}
	}
}
