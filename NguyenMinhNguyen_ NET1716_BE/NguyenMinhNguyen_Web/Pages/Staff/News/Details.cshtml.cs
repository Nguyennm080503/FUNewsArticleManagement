using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NguyenMinhNguyen_Web.Pages.Staff.News
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string NewsDetailApiUrl = "";
        private string NewsDetailTagApiUrl = "";

        public DetailsModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public NewsArticle NewsArticle { get; set; } = default!;
        public IList<TagResponse> TagResponses { get; set; }

        public class NewsDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public NewsArticle[] Value { get; set; }
        }

        public class TagResponse
        {
            [JsonProperty("$id")]
            public string Id { get; set; }

            public int TagId { get; set; }

            public string TagName { get; set; }

            public string Note { get; set; }

            public object NewsArticles { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                NewsDetailApiUrl = $"http://localhost:5137/odata/NewsArticle?$filter=NewsArticleId eq '{id}'&$expand=CreatedBy,Category";
                NewsDetailTagApiUrl = $"http://localhost:5137/get-tags?newID={id}";
                HttpResponseMessage response = await httpClient.GetAsync(NewsDetailApiUrl);
                HttpResponseMessage responseTag = await httpClient.GetAsync(NewsDetailTagApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var news = JsonConvert.DeserializeObject<NewsDetailResponse>(strData);
                    string strDataTag = await responseTag.Content.ReadAsStringAsync();
                    var jsonData = JObject.Parse(strDataTag);

                    var tagResponseList = new List<TagResponse>();
                    if (jsonData["$values"] is JArray values)
                    {
                        foreach (var item in values)
                        {
                            var tagResponse = item.ToObject<TagResponse>();
                            tagResponseList.Add(tagResponse);
                        }
                    }

                    TagResponses = tagResponseList;
                    NewsArticle = news.Value.FirstOrDefault();
                    return Page();
                }

                if (NewsArticle == null)
                {
                    return NotFound();
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }
    }
}
