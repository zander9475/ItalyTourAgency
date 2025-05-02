using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace ItalyTourAgency.Pages.Bookings
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;
        private readonly UserManager<User> _userManager;


        public DetailsModel(ItalyTourAgency.Models.ItalyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // --- Load booking ensuring user owns it and include related data ---
            var booking = await _context.Bookings
                .Include(b => b.TourInstance) 
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id); 

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;
            return Page();
        }

        // --- ADD THE CANCELLATION HANDLER ---
        // In Pages/Bookings/Details.cshtml.cs

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                var bookingToCancel = await _context.Bookings
                                            .Include(b => b.TourInstance)
                                            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

                if (bookingToCancel == null)
                {
                    TempData["CancellationMessage"] = "Booking not found or you do not have permission to cancel it.";
                    return RedirectToPage("./Index");
                }

                // --- Check Status ---
                // *** ADJUST these statuses if needed ("Confirmed", "Pending", etc.) ***
                if (bookingToCancel.Status != "Confirmed" && bookingToCancel.Status != "Pending")
                {
                    TempData["CancellationMessage"] = $"Booking with status '{bookingToCancel.Status}' cannot be cancelled.";
                    return RedirectToPage(new { id = id });
                }

                // --- Check TourInstance Exists ---
                if (bookingToCancel.TourInstance == null)
                {
                    Console.WriteLine($"CRITICAL ERROR: TourInstance is NULL for Booking ID {bookingToCancel.Id} during cancellation check.");
                    TempData["CancellationMessage"] = "Cannot check cancellation window: associated tour details are missing.";
                    return RedirectToPage(new { id = id });
                }
                var tourStartDate = bookingToCancel.TourInstance.StartDate;

                // --- **** ADD 7-DAY RULE CHECK **** ---
                var cancellationCutoffDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
                // Allow cancellation only if the start date is AFTER 7 days from now
                if (tourStartDate <= cancellationCutoffDate)
                {
                    TempData["CancellationMessage"] = $"Bookings can only be cancelled more than 7 days before the start date (cutoff: {cancellationCutoffDate:yyyy-MM-dd}).";
                    return RedirectToPage(new { id = id }); // Reload details page
                }
                // --- **** END OF 7-DAY RULE CHECK **** ---


                // Proceed with cancellation (rest of the logic remains the same)
                var tourInstance = bookingToCancel.TourInstance; // Already checked not null

                // Update slots
                tourInstance.BookedSlots = Math.Max(0, tourInstance.BookedSlots - bookingToCancel.GroupSize);
                _context.Attach(tourInstance).State = EntityState.Modified;

                // Update booking status
                bookingToCancel.Status = "Cancelled";
                _context.Attach(bookingToCancel).State = EntityState.Modified;

                // Save changes
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["CancellationMessage"] = "Booking successfully cancelled.";
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["CancellationMessage"] = "Could not cancel the booking due to a conflict. Please reload and try again.";
                    return RedirectToPage(new { id = id });
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"ERROR saving cancellation for booking {id}: {saveEx.ToString()}");
                    TempData["CancellationMessage"] = "An error occurred while saving the cancellation.";
                    return RedirectToPage(new { id = id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!!! UNEXPECTED ERROR in OnPostCancelAsync for booking {id}: {ex.ToString()} !!!!");
                TempData["CancellationMessage"] = "An unexpected error occurred during cancellation.";
                return RedirectToPage("./Index");
            }
        }
        // --- END OF HANDLER ---
    }
}