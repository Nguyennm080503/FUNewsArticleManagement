using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using DTOS;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NguyenMinhNguyen_Web.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string AccountUpdateApiUrl = "";
        private string AccountDetailApiUrl = "";

        public EditModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            AccountUpdateApiUrl = "http://localhost:5137/api/account/update";
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;
        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";
        public List<SelectListItem> RoleStatusOptions { get; set; }

        public class AccountDetailResponse
        {
            [JsonProperty("@odata.context")]
            public string Context { get; set; }

            public SystemAccount[] Value { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 0)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AccountDetailApiUrl = $"http://localhost:5137/odata/Account?$filter=AccountId eq {id}";
                HttpResponseMessage response = await httpClient.GetAsync(AccountDetailApiUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<AccountDetailResponse>(strData);
                    SystemAccount = account.Value.FirstOrDefault(); ;
                    RoleStatusOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Staff", Selected = SystemAccount.AccountRole == 1 },
                        new SelectListItem { Value = "2", Text = "Lecturer", Selected = SystemAccount.AccountRole == 2 }
        };
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accountUpdate = new AccountUpdate()
            {
                AccountEmail = SystemAccount.AccountEmail,
                AccountName = SystemAccount.AccountName,
                AccountRole = SystemAccount.AccountRole,
                AccountPassword = SystemAccount.AccountPassword,
                AccountId = SystemAccount.AccountId,
            };
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(accountUpdate), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(AccountUpdateApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageSuccess = "Update account successfully!";
                return Page();
            }

            MessageError = "There was an error during processing from the server.";
            return Page();
        }
    }
}
