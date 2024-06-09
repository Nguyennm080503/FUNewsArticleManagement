using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NguyenMinhNguyen_Web.Pages.Staff.Type
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string CategoryDeleteApiUrl = "";
        private string CategoryDetailApiUrl = "";

        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }
        public class CategoryDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public Category[] Value { get; set; }
        }

        public DeleteModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            CategoryDeleteApiUrl = "http://localhost:5137/api/category/delete/";
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        public string ErrorMessage { get; set; }

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

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClient.DeleteAsync($"{CategoryDeleteApiUrl}{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToPage("/Staff/Type/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await OnLoad(id);
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                ErrorMessage = apiResponse?.Message;
            }
            else
            {
                await OnLoad(id);
                ErrorMessage = "An error occurred while deleting the category.";
            }

            return Page();
        }
    }
}
