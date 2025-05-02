using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Bookings
{
    [Authorize(Roles = "Customer")]
    public class CreateBookingModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;
        private readonly UserManager<User> _userManager;

        public CreateBookingModel(ItalyTourAgency.Models.ItalyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking(); // Initialize to avoid null refs

        public Tour? CurrentTour { get; set; } // To display tour info

        [BindProperty]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please select a start date.")]
        public DateTime? SelectedDate { get; set; } // For the date picker

        // Holds available date ranges for display
        public List<(DateOnly StartDate, DateOnly EndDate)> AvailableDateRanges { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int tourId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Or handle appropriately
            }

            // Load Tour details and available dates
            var result = await LoadTourAndAvailableDates(tourId);
            if (!result)
            {
                // If tour not found or no dates, redirect or show message
                TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "Tour not found or no available dates.";
                // Consider redirecting to a more general tour listing page if tourId is invalid
                return RedirectToPage("/Tours/Index"); // Or back to details page if appropriate
            }

            // Pre-fill booking details
            Booking.TourId = tourId;
            Booking.UserId = user.Id;


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int tourId)
        {
            Console.WriteLine($"--- OnPostAsync ---");
            Console.WriteLine($"Route tourId parameter: {tourId}");
            Console.WriteLine($"Booking.TourId from model: {Booking.TourId}"); // Check if model binding worked as expected for the hidden field

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Ensure hidden fields are set even if Booking object is recreated
            Booking.TourId = tourId;
            Booking.UserId = user.Id;


            // Manual check for SelectedDate required because it's not directly part of Booking
            if (!SelectedDate.HasValue)
            {
                ModelState.AddModelError(nameof(SelectedDate), "Please select a start date.");
            }


            if (!ModelState.IsValid)
            {
                // *** ENSURE THIS LOGGING IS ACTIVE ***
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var value = ViewData.ModelState[modelStateKey];
                    if (value != null)
                    {
                        foreach (var error in value.Errors)
                        {
                            // Log the specific error and the key (property name) it belongs to
                            Console.WriteLine($"-- Key: {modelStateKey}, Error: {error.ErrorMessage}");
                        }
                    }
                }
                // *** END LOGGING ***

                await LoadTourAndAvailableDates(tourId);
                return Page();
            }

            // Convert selected DateTime to DateOnly for comparison
            var selectedDateOnly = DateOnly.FromDateTime(SelectedDate!.Value); // Non-null due to ModelState check

            // Find the TourInstance that matches the selected date
            var targetTourInstance = await _context.TourInstances
                .FirstOrDefaultAsync(ti =>
                    ti.TourId == tourId &&
                    ti.StartDate == selectedDateOnly &&
                    ti.Status == "Open"); // Use original logic

            if (targetTourInstance == null)
            {
                ModelState.AddModelError(nameof(SelectedDate), "The selected date is no longer available or valid for this tour.");
                await LoadTourAndAvailableDates(tourId); // Reload data
                return Page();
            }


            // Check capacity (optional but good practice)
            if (targetTourInstance.BookedSlots >= targetTourInstance.MaxCapacity)
            {
                ModelState.AddModelError(nameof(SelectedDate), "The selected tour date is now fully booked.");
                await LoadTourAndAvailableDates(tourId); // Reload data
                return Page();
            }

            // Capacity Check considering Group Size
            if ((targetTourInstance.BookedSlots + Booking.GroupSize) > targetTourInstance.MaxCapacity)
            {
                ModelState.AddModelError(nameof(Booking.GroupSize), $"Only {targetTourInstance.MaxCapacity - targetTourInstance.BookedSlots} spots left for the selected date.");
                await LoadTourAndAvailableDates(tourId); // Reload data
                return Page();
            }


            // Set the TourInstanceId on the Booking object
            Booking.TourInstanceId = targetTourInstance.Id;

            // Store necessary info in TempData for the payment page
            TempData["TourId"] = Booking.TourId;
            TempData["TourInstanceId"] = Booking.TourInstanceId;
            TempData["GroupSize"] = Booking.GroupSize;


            return RedirectToPage("./ProcessPayment");


        }


        // Helper method similar to EditModel's LoadEssentialData
        private async Task<bool> LoadTourAndAvailableDates(int tourId)
        {
            CurrentTour = await _context.Tours
                                .AsNoTracking() // Read-only operation
                                .FirstOrDefaultAsync(t => t.Id == tourId);


            if (CurrentTour == null)
            {
                TempData["ErrorMessage"] = "Tour not found.";
                return false; // Tour not found
            }


            var availableInstances = await _context.TourInstances
                .AsNoTracking()
                .Where(ti => ti.TourId == tourId && ti.Status == "Open" && ti.BookedSlots < ti.MaxCapacity) // Use original logic + capacity check
                .OrderBy(ti => ti.StartDate)
                .Select(ti => new { ti.StartDate, ti.EndDate }) // Select only needed fields
                .Distinct() // Ensure unique date ranges
                .ToListAsync();


            if (!availableInstances.Any())
            {
                TempData["ErrorMessage"] = "No available dates for this tour at the moment.";
                // We don't return false here, allow page to load but show message
            }


            AvailableDateRanges = availableInstances
                                    .Select(x => (x.StartDate, x.EndDate))
                                    .ToList();


            return true; // Data loaded successfully (even if no dates available)
        }


    }
}