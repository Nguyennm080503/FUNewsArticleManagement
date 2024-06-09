using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interface;

namespace NguyenMinhNguyen_Assignment2.Controllers
{
    public class TagController : ODataController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Authorize(policy: "Staff")]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var tags = await _tagService.GetAllTags();
            return Ok(tags);
        }

        [Authorize(policy: "Staff")]
        [HttpGet("get-tags")]
        public async Task<IActionResult> GetTagsForNewsArticle(string newID)
        {
            var tags = await _tagService.GetTagsForNewsArticle(newID);
            return Ok(tags);
        }
    }
}
