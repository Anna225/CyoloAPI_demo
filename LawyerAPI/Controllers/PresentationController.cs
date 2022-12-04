using LawyerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public PresentationController(LawyerDbContext context)
        {
            _context = context;
        }

        // GET: api/Presentations/20211155
        [HttpGet("GetByCourtCaseNo/{courtcaseno}")]
        public async Task<ActionResult<IEnumerable<dynamic>>?> GetAvailableLawyersByCourtCaseNo(int courtcaseno)
        {
            var lawyers = (from presentation in _context.Presentations
                           join lawyer in _context.Lawyers
                           on new { ID = presentation.LawyerId }
                           equals new { ID = lawyer.ID }
                           where (presentation.CourtCaseNo == courtcaseno) &&
                                   (presentation.Available == 1)
                           select new { presentation, lawyer });

            if (lawyers == null)
            {
                return null;
            }
            return await lawyers.ToListAsync();

        }

        // POST: api/Presentations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<string>> Presentation(Presentation presentation)
        {

            if (_context.Presentations == null)
            {
                return "There is a database error.";
            }
            if(_context.Presentations.Where(x => x.LawyerId == presentation.LawyerId).Where(x => x.CourtCaseNo == presentation.CourtCaseNo).Any())
            {
                return "You have already approved this court case.";
            }

            _context.Presentations.Add(presentation);
            await _context.SaveChangesAsync();
            return "Approved successfully.";

        }

        // GET: api/Presentations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presentation>> GetPresentation(int id)
        {

            if (_context.Presentations == null)
            {
                return NotFound();
            }
            var presentation = await _context.Presentations.FindAsync(id);

            if (presentation == null)
            {
                return NotFound();
            }

            return presentation;
        }
    }
}
