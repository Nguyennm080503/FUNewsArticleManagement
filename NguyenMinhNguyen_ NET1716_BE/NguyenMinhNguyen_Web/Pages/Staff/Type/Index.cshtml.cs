using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace NguyenMinhNguyen_Web.Pages.Staff.Type
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string TypeApiUrl = "";

        public IndexModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            TypeApiUrl = $"http://localhost:5137/odata/Category?$orderby=CategoryId desc";
        }

        public IList<Category> Category { get;set; } = default!;
        public class CategoryResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Category[] Value { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(TypeApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<CategoryResponse> (strData);
                    Category = categories.Value;
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
