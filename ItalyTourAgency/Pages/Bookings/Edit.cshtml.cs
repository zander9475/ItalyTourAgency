using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ItalyTourAgency.Pages.Bookings
{
    public class EditModel : PageModel
    {
        private readonly ItalyContext _context;
        private readonly UserManager<User> _userManager;

        public EditModel(ItalyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            MinDate = DateTime.Now.AddHours(48); // Initialize here
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public TourInstance OriginalTourInstance { get; set; } = default!;
        public DateTime MinDate { get; set; } = DateTime.Now.AddHours(48);

        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime? SelectedDate { get; set; }
        public List<(DateOnly StartDate, DateOnly EndDate)> AvailableDateRanges { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // Load essential data first
                await LoadEssentialData(id.Value);

                if (OriginalTourInstance == null)
                {
                    return NotFound("Tour instance not found");
                }

                return Page();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error loading booking data");
            }
        }

        private async Task LoadEssentialData(int bookingId)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.TourInstance)
                    .ThenInclude(ti => ti.Tour)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking?.TourInstance == null)
            {
                throw new Exception("Booking or tour instance not found");
            }

            OriginalTourInstance = booking.TourInstance;
            Booking = booking;

            // Ensure Tour is loaded
            if (OriginalTourInstance.Tour == null)
            {
                await _context.Entry(OriginalTourInstance)
                    .Reference(ti => ti.Tour)
                    .LoadAsync();
            }

            // Get available dates
            await LoadAvailableDates();
        }

        private async Task LoadAvailableDates()
        {
            if (OriginalTourInstance?.TourId == null) return;

            var minDateOnly = DateOnly.FromDateTime(MinDate);

            var dateRanges = await _context.TourInstances
                .AsNoTracking()
                .Where(ti => ti.TourId == OriginalTourInstance.TourId &&
                            ti.Id != OriginalTourInstance.Id &&
                            ti.StartDate >= minDateOnly &&
                            ti.BookedSlots < ti.MaxCapacity)
                .Select(ti => new { ti.StartDate, ti.EndDate })
                .Distinct()
                .ToListAsync();

            AvailableDateRanges = dateRanges
                .Select(x => (x.StartDate, x.EndDate))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadEssentialData(Booking.Id);
                return Page();
            }

            try
            {
                // Get current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                // Get fresh booking instance
                var originalBooking = await _context.Bookings
                    .Include(b => b.TourInstance)
                    .FirstOrDefaultAsync(b => b.Id == Booking.Id);

                if (originalBooking?.TourInstance == null)
                    return NotFound("Booking not found");

                // Fix 1: Proper date comparison
                if (!SelectedDate.HasValue)
                {
                    ModelState.AddModelError("", "Please select a date");
                    await LoadEssentialData(Booking.Id);
                    return Page();
                }

                // Convert to DateOnly at the start
                var selectedDateOnly = DateOnly.FromDateTime(SelectedDate.Value);
                var minDateOnly = DateOnly.FromDateTime(DateTime.Now.AddHours(48)); // Fix 2: Compare against current time

                // Fix 3: Proper date comparison
                if (selectedDateOnly < minDateOnly)
                {
                    ModelState.AddModelError("", "Date must be at least 48 hours in advance");
                    await LoadEssentialData(Booking.Id);
                    return Page();
                }

                // Find available tour instance
                var newTourInstance = await _context.TourInstances
                    .FirstOrDefaultAsync(ti =>
                        ti.TourId == originalBooking.TourInstance.TourId &&
                        ti.StartDate == selectedDateOnly &&  // Now comparing DateOnly with DateOnly
                        ti.BookedSlots < ti.MaxCapacity);

                if (newTourInstance == null)
                {
                    await LoadEssentialData(originalBooking.Id);
                    ModelState.AddModelError("", $"No availability on {selectedDateOnly:yyyy-MM-dd}. Available dates: " +
                        string.Join(", ", AvailableDateRanges.Select(x => $"{x.StartDate:yyyy-MM-dd} to {x.EndDate:yyyy-MM-dd}")));
                    return Page();
                }

                Console.WriteLine($"Selected: {SelectedDate} | Min: {MinDate} | SelectedDateOnly: {selectedDateOnly} | MinDateOnly: {minDateOnly}");

                // Update the booking
                originalBooking.TourInstanceId = newTourInstance.Id;
                originalBooking.TourId = newTourInstance.TourId; // Maintain tour reference
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving changes: {ex.Message}");
                await LoadEssentialData(Booking.Id);
                return Page();
            }
        }
    }
}