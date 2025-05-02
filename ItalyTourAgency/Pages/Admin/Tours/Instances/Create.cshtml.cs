using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ItalyTourAgency.Models;
using Microsoft.EntityFrameworkCore;

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
            // Remove TourInstance.MaxCapacity from the ModelState check initially
            // because we will set it manually. We still want to validate other fields.
            ModelState.Remove($"{nameof(TourInstance)}.{nameof(TourInstance.MaxCapacity)}");

            // Initialize default values BEFORE validating the rest of the model
            TourInstance.BookedSlots = 0;
            TourInstance.Status ??= "Open";

            if (!ModelState.IsValid)
            {
                // Log errors if needed (your existing code)
                // ...

                // Repopulate TourId SelectList before returning the Page
                ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name", TourInstance.TourId); // Pass selected value back
                return Page();
            }

            // Find the selected Tour using the TourId from the bound TourInstance
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == TourInstance.TourId); // Use FirstOrDefaultAsync for checking null
            if (tour == null)
            {
                ModelState.AddModelError("TourInstance.TourId", "Selected Tour does not exist.");
                ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Name", TourInstance.TourId);
                return Page();
            }

            // *** SET MaxCapacity HERE ***
            TourInstance.MaxCapacity = (int)tour.Capacity; // Set the instance capacity from the tour capacity

            // Now that MaxCapacity is set, you could potentially re-validate if needed,
            // but typically setting it from a trusted source (the Tour) is sufficient.

            // Assign the Tour navigation property (optional but good practice if needed later)
            // TourInstance.Tour = tour; // You already have this, which is good.

            _context.TourInstances.Add(TourInstance);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}