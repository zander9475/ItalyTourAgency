using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Admin.Tours
{
    public class DetailsModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public DetailsModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
            TourInstances = new List<TourInstance>();
        }

        [BindProperty]
        public Tour Tour { get; set; } = default!;
        public List<TourInstance> TourInstances { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.TourInstances)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            Tour = tour;
            TourInstances = tour.TourInstances.ToList();

            return Page();
        }
    }
}
