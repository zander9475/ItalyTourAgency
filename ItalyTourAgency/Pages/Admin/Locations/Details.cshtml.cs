using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages_Admin_Locations
{
    public class DetailsModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public DetailsModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FirstOrDefaultAsync(m => m.Id == id);

            if (location is not null)
            {
                Location = location;

                return Page();
            }

            return NotFound();
        }
    }
}
