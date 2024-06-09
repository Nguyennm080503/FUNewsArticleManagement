using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using DTOS;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NguyenMinhNguyen_Web.Pages.Staff.MyNews
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string NewsUpdateApiUrl = "";
        private string NewsyDetailApiUrl = "";
        private string TypeApiUrl = "";
        private string TagApiUrl = "";
        private string NewsDetailTagApiUrl = "";


        public EditModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            NewsUpdateApiUrl = "http://localhost:5137/api/news/update";
            TypeApiUrl = "http://localhost:5137/odata/Category";
            TagApiUrl = "http://localhost:5137/odata/Tag";
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        public IList<Category> Category { get; set; } = default!;
        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";
        public IList<Tag> Tags { get; set; } = default!;
        public List<SelectListItem> NewsStatusOptions { get; set; }
        public IList<TagsResponse> TagResponses { get; set; }

        public class NewsDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public NewsArticle[] Value { get; set; }
        }

        public class TagResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Tag[] Value { get; set; }
        }

        public class CategoryResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Category[] Value { get; set; }
        }
        public class TagsResponse
        {
            [JsonProperty("$id")]
            public string Id { get; set; }

            public int TagId { get; set; }

            public string TagName { get; set; }

            public string Note { get; set; }

            public object NewsArticles { get; set; }
        }
        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }


        public async Task OnLoad(string id)
        {
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            NewsyDetailApiUrl = $"http://localhost:5137/odata/NewsArticle?$filter=NewsArticleId eq '{id}'&$expand=CreatedBy,Category";
            NewsDetailTagApiUrl = $"http://localhost:5137/get-tags?newID={id}";
            HttpResponseMessage responseType = await httpClient.GetAsync(TypeApiUrl);
            HttpResponseMessage response = await httpClient.GetAsync(NewsyDetailApiUrl);
            HttpResponseMessage responseTag = await httpClient.GetAsync(TagApiUrl);
            HttpResponseMessage responseTags = await httpClient.GetAsync(NewsDetailTagApiUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.OK && responseType.StatusCode == System.Net.HttpStatusCode.OK && responseTag.StatusCode == System.Net.HttpStatusCode.OK && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();
                string strDataType = await responseType.Content.ReadAsStringAsync();
                var type = JsonConvert.DeserializeObject<CategoryResponse>(strDataType);
                var news = JsonConvert.DeserializeObject<NewsDetailResponse>(strData);
                string strDataTag = await responseTag.Content.ReadAsStringAsync();
                var tag = JsonConvert.DeserializeObject<TagResponse>(strDataTag);
                string strDataTags = await responseTags.Content.ReadAsStringAsync();
                var jsonData = JObject.Parse(strDataTags);

                var tagResponseList = new List<TagsResponse>();
                if (jsonData["$values"] is JArray values)
                {
                    foreach (var item in values)
                    {
                        var tagResponse = item.ToObject<TagsResponse>();
                        tagResponseList.Add(tagResponse);
                    }
                }

                TagResponses = tagResponseList;
                Tags = tag.Value;
                NewsArticle = news.Value.FirstOrDefault();
                Category = type.Value;
                NewsStatusOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Active", Selected = (bool)NewsArticle.NewsStatus },
            new SelectListItem { Value = "false", Text = "Inactive", Selected = (bool)!NewsArticle.NewsStatus }
        };
            }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                if (id == null)
                {
                    return NotFound();
                }
                else
                {
                    await OnLoad(id);
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<int> selectedTagIds = Request.Form["selectedTags"].Select(int.Parse).ToList();
            var accountID = HttpContext.Session.GetInt32("AccountID");
            var newsUpdate = new NewsUpdate()
            {
                CategoryId = NewsArticle.CategoryId,
                CreatedById = (short?)accountID,
                NewsArticleId = NewsArticle.NewsArticleId,
                NewsContent = NewsArticle.NewsContent,
                NewsTitle = NewsArticle.NewsTitle,
                Tags = selectedTagIds
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(newsUpdate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(NewsUpdateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await OnLoad(newsUpdate.NewsArticleId);
                MessageSuccess = "Update news article successfully!";
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await OnLoad(newsUpdate.NewsArticleId);
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                MessageError = apiResponse?.Message;
            }
            else
            {
                await OnLoad(newsUpdate.NewsArticleId);
                MessageError = "An error occurred while deleting the category.";
            }

            return Page();
        }

    }
}
