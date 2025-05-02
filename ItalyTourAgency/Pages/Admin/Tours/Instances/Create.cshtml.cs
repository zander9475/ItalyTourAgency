using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages_Admin_Tours_Instances
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
            ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name");

            return Page();
        }

        [BindProperty]
        public TourInstance TourInstance { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            TourInstance.BookedSlots = 0;
            TourInstance.Status ??= "Open";

            // Debug: Log ModelState errors
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"ModelState Error - Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name");
                return Page();
            }

            // Find the Tour and ensure it exists
            var tour = await _context.Tours.FindAsync(TourInstance.TourId);
            if (tour == null)
            {
                ModelState.AddModelError("TourInstance.TourId", "Selected Tour does not exist.");
                ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name");
                return Page();
            }

            // Assign the found Tour
            TourInstance.Tour = tour;

            _context.TourInstances.Add(TourInstance);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
