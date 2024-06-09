using BussinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace NguyenMinhNguyen_Web.Pages.Staff
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string AccountDetailApiUrl = "";
        
        public ProfileModel() 
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 1)
            {
                var id = HttpContext.Session.GetInt32("AccountID");
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AccountDetailApiUrl = $"http://localhost:5137/api/account/profile?accountID={id}";
                HttpResponseMessage response = await httpClient.GetAsync(AccountDetailApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<SystemAccount>(strData);
                    SystemAccount = account;
                    return Page();
                }

                if (SystemAccount == null)
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
    }
}
