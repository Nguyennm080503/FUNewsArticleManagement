using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using DTOS;
using Newtonsoft.Json;
using System.Text;

namespace NguyenMinhNguyen_Web.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string AccountCreateApiUrl = "";

        public CreateModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            AccountCreateApiUrl = "http://localhost:5137/api/account/create";
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 0)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

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

            var accountCreate = new AccountCreate()
            {
                AccountEmail = SystemAccount.AccountEmail,
                AccountName = SystemAccount.AccountName,
                AccountRole = SystemAccount.AccountRole,
                AccountPassword = SystemAccount.AccountPassword,
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(accountCreate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(AccountCreateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageSuccess = "Add account successfully!";
            }

            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseStatus>(strData);
                MessageError = apiResponse?.Message;
            }
            else
            {
                MessageError = "An error occurred while deleting the category.";
            }

            return Page();
        }
    }
}
