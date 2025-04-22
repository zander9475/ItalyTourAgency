using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItalyTourAgency.Areas.Identity.Pages.Account
{
    [Authorize]
    public class UserAccountModel : PageModel
    {
        private readonly UserManager<User> userManager;

        public User? appUser;

        public UserAccountModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public void OnGet()
        {
            var task = userManager.GetUserAsync(User);
            task.Wait();
            appUser = task.Result;
        }
    }
}
