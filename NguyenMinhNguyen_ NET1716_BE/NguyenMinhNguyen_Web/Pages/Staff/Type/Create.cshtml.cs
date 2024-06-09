using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using DTOS;
using Newtonsoft.Json;
using System.Text;

namespace NguyenMinhNguyen_Web.Pages.Staff.Type
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string CategoryCreateApiUrl = "";

        public CreateModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            CategoryCreateApiUrl = "http://localhost:5137/api/category/create";
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";
        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var categoryCreate = new CategoryCreate()
            {
                CategoryName = Category.CategoryName,
                CategoryDesciption = Category.CategoryDesciption,
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(categoryCreate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(CategoryCreateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageSuccess = "Add category successfully!";
                return Page();
            }

            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                MessageError = apiResponse?.Message;
            }
            else
            {
                MessageError = "There was an error during processing from the server.";
            }
            return Page();
        }
    }
}
