using ItalyTourAgency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.EntityFrameworkCore;

namespace ItalyTourAgency.Pages.Bookings
{
    [Authorize(Roles = "Customer")]
    public class ProcessPaymentModel : PageModel
    {
        private readonly ItalyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProcessPaymentModel> _logger;

        [BindProperty]
        public string? StripeToken { get; set; }

        public string TourDates { get; set; } = string.Empty;
        public int GroupSize { get; set; }
        public decimal TotalPrice { get; set; }

        public ProcessPaymentModel(ItalyContext context, UserManager<User> userManager, ILogger<ProcessPaymentModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            // Retrieve from TempData
            if (!TempData.TryGetValue("TourInstanceId", out var instanceIdObj) ||
                !TempData.TryGetValue("GroupSize", out var groupSizeObj))
            {
                return RedirectToPage("./CreateBooking");
            }

            var tourInstanceId = (int)instanceIdObj!;
            var groupSize = (int)groupSizeObj!;

            var tourInstance = await _context.TourInstances
                .Include(ti => ti.Tour)
                .FirstOrDefaultAsync(ti => ti.Id == tourInstanceId);

            if (tourInstance == null)
            {
                ModelState.AddModelError("", "Tour instance not found");
                return await OnGet();
            }

            if (!await _context.Tours.AnyAsync(t => t.Id == tourInstance.TourId))
            {
                ModelState.AddModelError("", "Associated tour not found");
                return await OnGet();
            }

            if (tourInstance?.Tour == null)
            {
                return NotFound();
            }

            // Set display values
            TourDates = $"{tourInstance.StartDate:d} to {tourInstance.EndDate:d}";
            GroupSize = groupSize;
            TotalPrice = (tourInstance.Tour.Price ?? 0) * groupSize;

            // Store in TempData again for post
            TempData.Keep();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Keep TempData before any potential redirects
            TempData.Keep("TourInstanceId");
            TempData.Keep("GroupSize");

            // Debugging statements
            Console.WriteLine($"Token received: {!string.IsNullOrEmpty(StripeToken)}");
            Console.WriteLine($"TempData TourInstanceId: {TempData["TourInstanceId"]}");
            Console.WriteLine($"TempData GroupSize: {TempData["GroupSize"]}");

            if (string.IsNullOrEmpty(StripeToken))
            {
                ModelState.AddModelError("", "Payment token is missing");
                return await OnGet();
            }

            if (!TempData.TryGetValue("TourInstanceId", out var instanceIdObj) ||
                !TempData.TryGetValue("GroupSize", out var groupSizeObj))
            {
                return RedirectToPage("/Bookings/CreateBooking");
            }

            var tourInstanceId = (int)instanceIdObj!;
            var groupSize = (int)groupSizeObj!;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Keep TempData again after reading
            TempData.Keep("TourInstanceId");
            TempData.Keep("GroupSize");

            var tourInstance = await _context.TourInstances
                .Include(ti => ti.Tour)
                .FirstOrDefaultAsync(ti => ti.Id == tourInstanceId);

            if (tourInstance?.Tour == null)
            {
                ModelState.AddModelError("", "Tour instance no longer exists");
                return await OnGet();
            }

            TotalPrice = (tourInstance.Tour.Price ?? 0) * groupSize;

            try
            {
                StripeConfiguration.ApiKey = "sk_test_51RJTA9Fk4dcR1IVJnENjnya3wZgvwY5uHgD7HbfDv1niAltKPUwkF49aLoooVbzS5xJTxy5he8Ab6mHoDMYjMvFG00h1JPvjet";

                var options = new ChargeCreateOptions
                {
                    Amount = (long)(TotalPrice * 100),
                    Currency = "usd",
                    Source = StripeToken,
                    Description = $"Booking for {user.Email}"
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                if (charge.Paid)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var booking = new Booking
                        {
                            UserId = user.Id,
                            TourId = tourInstance.TourId,
                            TourInstanceId = tourInstanceId,
                            GroupSize = groupSize,
                            TotalPrice = TotalPrice,
                            Status = "Confirmed",
                            PaymentDate = DateTime.UtcNow,
                            PaymentProcessed = true
                        };

                        _context.Bookings.Add(booking);

                        // Update available slots
                        tourInstance.BookedSlots += groupSize;

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return RedirectToPage("./Confirmation", new { id = booking.Id });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Failed to save booking: " + ex.Message);
                        _logger.LogError(ex, "Database save failed");
                        return Page();
                    }
                }
            }
            catch (StripeException e)
            {
                ModelState.AddModelError("", $"Payment failed: {e.StripeError.Message}");
                _logger.LogError(e, "Stripe payment failed");
            }

            return Page();
        }
    }
}