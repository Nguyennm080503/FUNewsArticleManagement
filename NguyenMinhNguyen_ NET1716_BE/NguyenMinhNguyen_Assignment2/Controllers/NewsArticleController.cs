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
    [Route("api/news")]
    [ApiController]
    public class NewsArticleController : ODataController
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticleController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [Authorize(policy: "Staff")]
        [EnableQuery]
        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            var news = await _newsArticleService.GetAllNewsArticle();
            return Ok(news);
        }

        [Authorize(policy: "Staff")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] NewsCreate newsCreate)
        {
            var news = _newsArticleService.GetAllNewsArticle().Result.FirstOrDefault(x => x.NewsTitle == newsCreate.NewsTitle);
            if (news == null)
            {
                await _newsArticleService.CreateNewsArticle(newsCreate);
                return Ok();
            }
            else
            {
                return BadRequest(new ApiResponseStatus(404, "NewsArticle is existed!"));
            }  
        }

        [Authorize(policy: "Staff")]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromRoute] string newsID)
        {
            await _newsArticleService.DeleteNewsArticle(newsID);
            return Ok();
        }

        [Authorize(policy: "Staff")]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] NewsUpdate newsUpdate)
        {
            var newsTitle = _newsArticleService.GetAllNewsArticle().Result.FirstOrDefault(x => x.NewsArticleId == newsUpdate.NewsArticleId).NewsTitle;
            if (newsUpdate.NewsTitle == newsTitle)
            {
                await _newsArticleService.UpdateNewsArticle(newsUpdate);
                return Ok();
            }
            else
            {
                var category = _newsArticleService.GetAllNewsArticle().Result.FirstOrDefault(x => x.NewsTitle == newsUpdate.NewsTitle);
                if (category != null)
                {
                    return BadRequest(new ApiResponseStatus(404, "NewsArticle is existed!"));
                }
                else
                {
                    await _newsArticleService.UpdateNewsArticle(newsUpdate);
                    return Ok();
                }
            }
        }

        [Authorize(policy: "Staff")]
        [HttpPut("change_status")]
        public async Task<IActionResult> Put([FromRoute] string newsID, [FromBody] int status)
        {
            await _newsArticleService.ChangeStatusNewsArticle(newsID, status);
            return Ok();
        }

    }
}
