using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages.Public.Tours
{
    public class IndexModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public IndexModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public IList<Tour> Tour { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Tour = await _context.Tours.ToListAsync();
        }
    }
}
