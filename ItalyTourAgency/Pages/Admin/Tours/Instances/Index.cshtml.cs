using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItalyTourAgency.Models;

namespace ItalyTourAgency.Pages_Admin_Tours_Instances
{
    public class IndexModel : PageModel
    {
        private readonly ItalyTourAgency.Models.ItalyContext _context;

        public IndexModel(ItalyTourAgency.Models.ItalyContext context)
        {
            _context = context;
        }

        public IList<TourInstance> TourInstance { get;set; } = default!;

        public async Task OnGetAsync()
        {
            TourInstance = await _context.TourInstances
                .Include(t => t.Tour).ToListAsync();
        }
    }
}
