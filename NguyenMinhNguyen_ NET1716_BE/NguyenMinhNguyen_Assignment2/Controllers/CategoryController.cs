using DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NguyenMinhNguyen_Assignment2.MessageStatusResponse;
using Service.Implement;
using Service.Interface;

namespace NguyenMinhNguyen_Assignment2.Controllers
{
    [Route("api/category/")]
    [ApiController]
    public class CategoryController : ODataController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(policy: "Staff")]
        [EnableQuery]
        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            var category = await _categoryService.GetAllCategory();
            return Ok(category);
        }

        [Authorize(policy: "Staff")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CategoryCreate categoryCreate)
        {
            var cate = _categoryService.GetAllCategory().Result.FirstOrDefault(x => x.CategoryName == categoryCreate.CategoryName);
            if (cate == null)
            {
                await _categoryService.CreateCategory(categoryCreate);
                return Ok();
            }
            else
            {
                return BadRequest(new ApiResponseStatus(404, "Category is existed!"));
            }
        }

        [Authorize(policy: "Staff")]
        [HttpDelete("delete/{categoryID}")]
        public async Task<IActionResult> Delete([FromRoute] int categoryID)
        {
            bool check = await _categoryService.DeleteCategory(categoryID);
            if (!check)
            {
                return BadRequest(new ApiResponseStatus(404, "The item is already stored in a news article cannot delete."));
            }
            return Ok(check);
        }

        [Authorize(policy: "Staff")]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdate categoryUpdate)
        {
            var categoryName = _categoryService.GetAllCategory().Result.FirstOrDefault(x => x.CategoryId == categoryUpdate.CategoryId).CategoryName;
            if (categoryUpdate.CategoryName == categoryName)
            {
                await _categoryService.UpdateCategory(categoryUpdate);
                return Ok();
            }
            else
            {
                var category =  _categoryService.GetAllCategory().Result.FirstOrDefault(x => x.CategoryName == categoryUpdate.CategoryName);
                if (category != null)
                {
                    return BadRequest(new ApiResponseStatus(404, "Category is existed!"));
                }
                else
                {
                    await _categoryService.UpdateCategory(categoryUpdate);
                    return Ok();
                }
            }
        }
    }
}
