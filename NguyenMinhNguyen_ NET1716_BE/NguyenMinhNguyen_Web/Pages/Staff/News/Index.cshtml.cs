using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace NguyenMinhNguyen_Web.Pages.Staff.News
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string NewsApiUrl = "";

        public IndexModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            NewsApiUrl = $"http://localhost:5137/odata/NewsArticle?$orderby=CreatedDate desc&$expand=CreatedBy,Category";
        }

        public IList<NewsArticle> NewsArticle { get;set; } = default!;

        public class NewsResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public NewsArticle[] Value { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(NewsApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var news = JsonConvert.DeserializeObject<NewsResponse>(strData);
                    NewsArticle = news.Value;
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Error");
                }
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }
    }
}
