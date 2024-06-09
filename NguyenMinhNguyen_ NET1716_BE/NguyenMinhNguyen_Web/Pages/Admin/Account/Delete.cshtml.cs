using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace NguyenMinhNguyen_Web.Pages.Account
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string AccountDeleteApiUrl = "";
        private string AccountDetailApiUrl = "";

        public class ApiResponseStatus
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }

        public class AccountDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public SystemAccount[] Value { get; set; }
        }

        public DeleteModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            AccountDeleteApiUrl = "http://localhost:5137/api/account/delete/";
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        public string ErrorMessage {  get; set; }



        public async Task OnLoad(short? id)
        {
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            AccountDetailApiUrl = $"http://localhost:5137/odata/Account?$filter=AccountId eq {id}";
            HttpResponseMessage response = await httpClient.GetAsync(AccountDetailApiUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var account = JsonConvert.DeserializeObject<AccountDetailResponse>(strData);
                SystemAccount = account.Value.FirstOrDefault();
            }
        }
        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 0)
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
            HttpResponseMessage response = await httpClient.DeleteAsync($"{AccountDeleteApiUrl}{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToPage("/Admin/Account/Index");
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
                ErrorMessage = "An error occurred while deleting the account.";
            }

            return Page();
        }
    }
}
