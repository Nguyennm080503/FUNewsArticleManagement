using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace NguyenMinhNguyen_Web.Pages.Staff.News
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string NewsDeleteApiUrl = "";
        private string NewsDetailApiUrl = "";

        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }

        public DeleteModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            NewsDeleteApiUrl = "http://localhost:5137/api/news/delete/";
        }

        public class NewsDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public NewsArticle[] Value { get; set; }
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        public string ErrorMessage { get; set; }

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
                HttpResponseMessage response = await httpClient.GetAsync(NewsDetailApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var news = JsonConvert.DeserializeObject<NewsDetailResponse>(strData);
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClient.DeleteAsync($"{NewsDeleteApiUrl}{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToPage("/Staff/News/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                ErrorMessage = apiResponse?.Message;
            }
            else
            {
                ErrorMessage = "An error occurred while deleting the news article.";
            }

            return Page();
        }
    }
}
