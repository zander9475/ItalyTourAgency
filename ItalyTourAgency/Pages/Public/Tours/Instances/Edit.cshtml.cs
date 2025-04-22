using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Tours.Instances
{
    public class EditModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public EditModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TourInstance TourInstance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourinstance =  await _context.TourInstances.FirstOrDefaultAsync(m => m.Id == id);
            if (tourinstance == null)
            {
                return NotFound();
            }
            TourInstance = tourinstance;
           ViewData["TourId"] = new SelectList(_context.Tours, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TourInstance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TourInstanceExists(TourInstance.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TourInstanceExists(int id)
        {
            return _context.TourInstances.Any(e => e.Id == id);
        }
    }
}
