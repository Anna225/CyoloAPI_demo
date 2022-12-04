using LawyerAPI.Helper;
using LawyerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public CustomController(LawyerDbContext context)
        {
            _context = context;
        }

        private bool checkURL(string currentUrl)
        {
            if (currentUrl.Contains(".azurewebsites.net"))
            {
                return false;
            }
            return true;
        }

        // GET: api/Custom/CourtCaseByDateAndCourtName/2022-10-03/democourtname
        [HttpGet("CourtCaseByDateAndCourtName/{date}/{courtname}")]
        public async Task<ActionResult<List<CourtCaseAgenda?>>> GetCourtCaseByDateAndCourtName(string date, string courtname)
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(x => x.HearingDate == date && (x.HearingGeneral!.Contains(courtname)))
                .GroupBy(x => x.HearingGeneral)
                .Select(m => m.FirstOrDefault())
                .ToListAsync();            
        }

        // GET: api/Custom/CourtCaseBydateAndEmail/2022-10-03/demoemail
        [HttpGet("CourtCaseByDateAndEmail/{date}/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndEmail(string date, string lawyeremail)
        {
            var courtcases = (from courtcase in _context.CourtCaseAgenda
                              join assign in _context.AssignedLawyers
                              on courtcase.ID equals assign.CourtCaseId
                              join lawyer in _context.Lawyers
                              on assign.Name equals lawyer.Name
                              where lawyer.Email == lawyeremail &&
                                      (courtcase.HearingDate == date)
                              orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                              select new { courtcase }).Distinct();

            if (courtcases == null)
            {
                return NotFound();
            }
            return await courtcases.ToListAsync();
        }

        // GET: api/Custom/CourtCaseByEmail/demoemail
        [HttpGet("CourtCaseByEmail/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByEmail(string lawyeremail)
        {
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                              join assign in _context.AssignedLawyers
                              on courtcase.ID equals assign.CourtCaseId
                              join lawyer in _context.Lawyers
                              on assign.Name equals lawyer.Name
                              where lawyer.Email == lawyeremail
                              orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                              select new { courtcase }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/LawyersByCourtCaseId/234
        [HttpGet("LawyersByCourtCaseId/{courtcaseid}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetLawyersByCourtCaseId(string courtcaseid)
        {
            var queryParam = System.Uri.UnescapeDataString(courtcaseid);
            var lawyers = (from courtcase in _context.CourtCaseAgenda
                           join assign in _context.AssignedLawyers
                           on courtcase.ID equals assign.CourtCaseId
                           join lawyer in _context.Lawyers
                           on assign.Name equals lawyer.Name
                           where courtcase.CourtCaseNo == courtcaseid
                           orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                           select new { lawyer }).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }
        
        // GET: api/Custom/CourtCaseByDateAndCourtName/2022-10-03/democourtname
        [HttpGet("AllCourtCasesByDate/{date}")]
        public async Task<ActionResult<List<CourtCaseAgenda?>>> GetAllCourtCasesByDate(string date)
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(x => x.HearingDate == date)
                .GroupBy(x => x.CourtCaseNo)
                .Select(m => m.FirstOrDefault())
                .ToListAsync();            
        }

        // GET: api/Custom/CourtCaseByDateAndEmail/2022-10-03/name
        [HttpGet("CourtCaseByDateAndName/{date}/{lawyername}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndName(string date, string lawyername)
        {
            var courtcases = (from courtcase in _context.CourtCaseAgenda
                              join assign in _context.AssignedLawyers
                              on courtcase.ID equals assign.CourtCaseId
                              where assign.Name == lawyername.ToLower() &&
                                      (courtcase.HearingDate == date)
                              orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                              select new { courtcase });

            if (courtcases == null)
            {
                return NotFound();
            }
            return await courtcases.ToListAsync();
        }

        // GET: api/Custom/CourtCaseBydateAndPhone/2022-10-03/phonenumber
        [HttpGet("CourtCaseByDateAndPhone/{date}/{phone}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCourtCaseByDateAndPhone(string date, string phone)
        {
            var courtcases = (from courtcase in _context.CourtCaseAgenda
                              join assign in _context.AssignedLawyers
                              on courtcase.ID equals assign.CourtCaseId
                              join lawyer in _context.Lawyers
                              on assign.Name equals lawyer.Name
                              where (lawyer.Phone == phone || lawyer.Fax == phone) &&
                                      (courtcase.HearingDate == date)
                              orderby courtcase.HearingDate descending, courtcase.HearingTime descending
                              select new { courtcase });

            if (courtcases == null)
            {
                return NotFound();
            }
            return await courtcases.ToListAsync();
        }



        // GET: api/GetLawyerByEmail/admin@gmail.com
        [HttpGet("GetLawyerByEmail/{email}")]
        public async Task<ActionResult<Lawyer>> GetLawyerByEmail(string email)
        {
            if (_context.Lawyers == null)
            {
                return NotFound();
            }
            var lawyer = await _context.Lawyers.Where(z => z.Email == email).FirstOrDefaultAsync();

            if (lawyer == null)
            {
                return NotFound();
            }

            return lawyer;
        }

        // GET: api/Custom/AllJurisdictions
        [HttpGet("AllJurisdictions")]
        public async Task<ActionResult<List<string?>>> GetAllJurisdictions()
        {
            return await _context.CourtCaseAgenda.AsNoTracking()
                .Where(y => !string.IsNullOrEmpty(y.HearingGeneral))
                .OrderBy(o => o.HearingGeneral)
                .Select(m => m.HearingGeneral)
                .Distinct()
                .ToListAsync();
        }

    }
}
