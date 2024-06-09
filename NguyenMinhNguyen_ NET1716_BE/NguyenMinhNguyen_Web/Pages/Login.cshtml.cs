using DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace NguyenMinhNguyen_Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient httpClient = null;
        private string LoginApiUrl = " ";

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel()
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            LoginApiUrl = "http://localhost:5137/api/login";
        }
        public async Task<IActionResult> OnPost()
        {
            var loginDto = new LoginDto()
            {
                AccountEmail = Email,
                AccountPassword = Password,
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(LoginApiUrl, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginTokenDto>(strData);

                if (loginResponse != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(loginResponse.Token) as JwtSecurityToken;

                    var accountId = jsonToken.Claims.First(claim => claim.Type == "AccountId").Value;
                    var roleId = jsonToken.Claims.First(claim => claim.Type == "AccountRole").Value;

                    HttpContext.Session.SetInt32("AccountID",int.Parse(accountId));
                    HttpContext.Session.SetInt32("RoleID", int.Parse(roleId));
                    HttpContext.Session.SetString("Token", loginResponse.Token);
                    if (int.Parse(roleId) == 1)
                    {
                        return RedirectToPage("/Staff/Profile");
                    }
                    else if(int.Parse(roleId) == 0)
                    {
                        return RedirectToPage("/Admin/Dashboard");
                    }
                    else if(int.Parse(roleId) == 2)
                    {
                        return RedirectToPage("/Lecturer");
                    }
                    else
                    {
                        return RedirectToPage("/Permission");
                    }
                }
                else
                {
                    ErrorMessage = "Email or password is incorrect.";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = "There was an error during processing from the server.";
                return Page();
            }
        }

    }
}
