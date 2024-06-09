using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenMinhNguyen_Web.Pages
{
    public class LecturerModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 2)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("Permission");
            }
        }
    }
}
