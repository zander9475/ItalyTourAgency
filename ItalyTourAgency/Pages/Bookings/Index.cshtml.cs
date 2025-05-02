using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ItalyTourAgency.Pages_Bookings
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ItalyTourAgency.Models.ItalyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Booking = new List<Booking>();
                return;
            }

            // Force fresh data
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            Booking = await _context.Bookings
                .Include(b => b.TourInstance)
                    .ThenInclude(ti => ti.Tour)
                .Where(b => b.UserId == user.Id && b.Status != "Cancelled")
                .AsNoTracking()  // Add this for fresh data
                .ToListAsync();
        }
    }
}
