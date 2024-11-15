using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TechTestKLG.Pages
{
    [Authorize]
    public class ActivityModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
