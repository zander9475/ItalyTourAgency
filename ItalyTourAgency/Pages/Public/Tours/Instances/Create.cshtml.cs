using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Tours.Instances
{
    public class CreateModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public CreateModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public TourInstance TourInstance { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TourInstances.Add(TourInstance);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
