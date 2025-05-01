using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ItalyTourAgency.Pages.Bookings
{
    public class ConfirmationModel : PageModel
    {
        private readonly ItalyContext _context;

        public ConfirmationModel(ItalyContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            Booking = await _context.Bookings
                .Include(b => b.TourInstance)
                    .ThenInclude(ti => ti.Tour)
                 .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Booking == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}