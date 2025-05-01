using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Admin.Locations
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ItalyContext _context;

        public CreateModel(ItalyContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Location = new Location { Status = "Draft" };
            return Page();
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Location.Status ??= "Draft";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Locations.Add(Location);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
