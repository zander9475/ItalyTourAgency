using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Tours.Instances
{
    public class DeleteModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public DeleteModel(ItalyTourAgency.Models.ItalyContext context)
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

            var tourinstance = await _context.TourInstances.FirstOrDefaultAsync(m => m.Id == id);

            if (tourinstance is not null)
            {
                TourInstance = tourinstance;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourinstance = await _context.TourInstances.FindAsync(id);
            if (tourinstance != null)
            {
                TourInstance = tourinstance;
                _context.TourInstances.Remove(TourInstance);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
