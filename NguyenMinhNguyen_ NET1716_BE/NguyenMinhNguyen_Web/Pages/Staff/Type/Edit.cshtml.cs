using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using DTOS;
using System.Text;

namespace NguyenMinhNguyen_Web.Pages.Staff.Type
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string CategoryUpdateApiUrl = "";
        private string CategoryDetailApiUrl = "";

        public EditModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            CategoryUpdateApiUrl = "http://localhost:5137/api/category/update";
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";

        public class CategoryDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Category[] Value { get; set; }
        }

        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }

        public async Task OnLoad(short? id)
        {
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            CategoryDetailApiUrl = $"http://localhost:5137/odata/Category?$filter=CategoryId eq {id}";
            HttpResponseMessage response = await httpClient.GetAsync(CategoryDetailApiUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryDetailResponse>(strData);
                Category = category.Value.FirstOrDefault();
            }
        }

        public async Task<IActionResult> OnGetAsync(short? id)
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
                await OnLoad(Category.CategoryId);
                return Page();
            }

            var categoryUpdate = new CategoryUpdate()
            {
                CategoryName = Category.CategoryName,
                CategoryDesciption = Category.CategoryDesciption,
                CategoryId = Category.CategoryId,
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(categoryUpdate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(CategoryUpdateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await OnLoad(Category.CategoryId);
                MessageSuccess = "Update category successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await OnLoad(Category.CategoryId);
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                MessageError = apiResponse?.Message;
            }
            else
            {
                await OnLoad(Category.CategoryId);
                MessageError = "There was an error during processing from the server.";
            }
            return Page();
        }

    }
}
