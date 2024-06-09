using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace NguyenMinhNguyen_Web.Pages.Staff.MyNews
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
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;

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
                var accountID = HttpContext.Session.GetInt32("AccountID");
                NewsApiUrl = $"http://localhost:5137/odata/NewsArticle?$filter=CreatedById eq {accountID}&$orderby=CreatedDate desc&$expand=CreatedBy,Category";
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
