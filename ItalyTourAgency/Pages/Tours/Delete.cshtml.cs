using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Tours
{
    public class DeleteModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public DeleteModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tour Tour { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FirstOrDefaultAsync(m => m.Id == id);

            if (tour is not null)
            {
                Tour = tour;

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

            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                Tour = tour;
                _context.Tours.Remove(Tour);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
