using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Admin.Tours
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public CreateModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Tour = new Tour { Status = "Draft" };
            return Page();
        }

        [BindProperty]
        public Tour Tour { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Tour.Status ??= "Draft";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Tours.Add(Tour);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
