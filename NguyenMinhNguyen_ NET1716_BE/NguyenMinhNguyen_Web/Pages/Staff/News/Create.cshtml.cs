using BussinessObjects.Models;
using DTOS;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NguyenMinhNguyen_Web.Pages.Staff.News
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string NewsCreateApiUrl = "";
        private string TypeApiUrl = "";
        private string TagApiUrl = "";

        public class TagResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Tag[] Value { get; set; }
        }
        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }
        public class CategoryResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Category[] Value { get; set; }
        }

        public CreateModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            NewsCreateApiUrl = "http://localhost:5137/api/news/create";
            TypeApiUrl = "http://localhost:5137/odata/Category";
            TagApiUrl = "http://localhost:5137/odata/Tag";
        }

        private async Task LoadData()
        {
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage responseType = await httpClient.GetAsync(TypeApiUrl);
            HttpResponseMessage responseTag = await httpClient.GetAsync(TagApiUrl);

            if (responseType.StatusCode == System.Net.HttpStatusCode.OK && responseTag.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strDataType = await responseType.Content.ReadAsStringAsync();
                var type = JsonConvert.DeserializeObject<CategoryResponse>(strDataType);
                string strDataTag = await responseTag.Content.ReadAsStringAsync();
                var tag = JsonConvert.DeserializeObject<TagResponse>(strDataTag);

                Tags = tag.Value;
                Categories = type.Value;
            }
        }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                await LoadData();
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public IList<Category> Categories { get; set; } = default;
        [BindProperty]
        public List<int> SelectedTags { get; set; } = new List<int>();
        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";

        public async Task<IActionResult> OnPostAsync()
        {
            var newsCreate = new NewsCreate()
            {
                CategoryId = NewsArticle.CategoryId,
                CreatedById = (short?)HttpContext.Session.GetInt32("AccountID"),
                NewsContent = NewsArticle.NewsContent,
                NewsTitle = NewsArticle.NewsTitle,
                Tags = SelectedTags
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(newsCreate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(NewsCreateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await LoadData();
                MessageSuccess = "Add news article successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await LoadData();
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                MessageError = apiResponse?.Message;
            }
            else
            {
                await LoadData();
                MessageError = "An error occurred while deleting the category.";
            }

            return Page();
        }
    }
}
