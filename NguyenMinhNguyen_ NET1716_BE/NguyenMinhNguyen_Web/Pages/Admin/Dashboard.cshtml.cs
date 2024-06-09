using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;

namespace NguyenMinhNguyen_Web.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5137/odata/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        [BindProperty]
        public DateTime StartDate { get; set; }

        [BindProperty]
        public DateTime EndDate { get; set; }

        public List<ReportData> ReportDatas { get; set; }

        public class ReportData
        {
            [JsonProperty("CreatedDate")]
            public DateTime Date { get; set; }
            public int TotalNews { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start Date cannot be greater than End Date.");
                return Page();
            }

            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var filter = $"$filter=CreatedDate ge {StartDate:yyyy-MM-dd} and CreatedDate le {EndDate:yyyy-MM-dd}";
            var apply = $"$apply=groupby((CreatedDate), aggregate($count as TotalNews))";
            var requestUrl = $"NewsArticle?{filter}&{apply}";

            var absoluteUri = new Uri(_httpClient.BaseAddress, requestUrl);

            HttpResponseMessage response = await _httpClient.GetAsync(absoluteUri);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var jsonData = JObject.Parse(strData);

                var reportDataList = new List<ReportData>();
                if (jsonData["$values"] is JArray values)
                {
                    foreach (var item in values)
                    {
                        var reportData = item.ToObject<ReportData>();
                        reportDataList.Add(reportData);
                    }
                }

                ReportDatas = reportDataList;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error retrieving report data.");
            }

            return Page();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 0)
            {
                ReportDatas = null;
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }
    }
}
