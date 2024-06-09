using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace NguyenMinhNguyen_Web.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string AccountApiUrl = "";

        public class AccountResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public SystemAccount[] Value { get; set; }
        }
        public IndexModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            AccountApiUrl = $"http://localhost:5137/odata/Account?$orderby=AccountId desc";
        }

        public IList<SystemAccount> SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if(HttpContext.Session.GetInt32("RoleID") == 0)
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(AccountApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var accountResponse = JsonConvert.DeserializeObject<AccountResponse>(strData);
                    SystemAccount = accountResponse.Value.ToList();
                    return Page();
                }
                else
                {
                    return RedirectToPage("Error");
                }
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }
    }
}
