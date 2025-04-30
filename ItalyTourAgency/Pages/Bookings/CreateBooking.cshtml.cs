using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItalyTourAgency.Pages.Bookings
{
    [Authorize(Roles = "Customer")]
    public class CreateBookingModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public Booking Booking { get; set; } = null!;

        public List<SelectListItem> AvailableTourInstances { get; set; } = new();

        public CreateBookingModel(ItalyTourAgency.Models.ItalyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(int tourId)
        {
            // Initialize Booking
            Booking = new Booking
            {
                TourId = tourId,
                UserId = (await _userManager.GetUserAsync(User))?.Id ?? string.Empty
            };

            // Get current tour
            var tour = await _context.Tours
                .Include(t => t.TourInstances)
                .FirstOrDefaultAsync(t => t.Id == tourId);

            if (tour?.TourInstances == null)
            {
                return NotFound();
            }

            // Filter available tour instances
            var availableInstances = tour.TourInstances
                .Where(ti => ti.Status == "Open")
                .Select(ti => new SelectListItem
                {
                    Value = ti.Id.ToString(),
                    Text = $"{ti.StartDate:d} to {ti.EndDate:d}"
                })
                .ToList();

            if (!availableInstances.Any())
            {
                TempData["ErrorMessage"] = "No available dates for this tour";
                return RedirectToPage("/Tours/Details", new { id = tourId });
            }

            ViewData["TourInstanceId"] = new SelectList(availableInstances, "Value", "Text");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int tourId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Set IDs for hidden fields
            Booking ??= new Booking();
            Booking.TourId = tourId;
            Booking.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                /* 
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"ModelState Error - Key: {key}, Error: {error.ErrorMessage}");
                    }
                } 
                */

                await OnGet(tourId); // Repopulate dropdown
                return Page();
            }

            // Store in TempData for payment page
            TempData["TourId"] = tourId;
            TempData["TourInstanceId"] = Booking.TourInstanceId;
            TempData["GroupSize"] = Booking.GroupSize;

            return RedirectToPage("./ProcessPayment");
        }
    }
}
