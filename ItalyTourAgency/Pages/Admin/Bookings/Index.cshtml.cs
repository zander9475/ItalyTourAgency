using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItalyTourAgency.Pages_Admin_Bookings
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public IndexModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _context.Bookings
                .Include(b => b.TourInstance)
                .ThenInclude(ti => ti.Tour)
                .ToListAsync();
        }
    }
}
