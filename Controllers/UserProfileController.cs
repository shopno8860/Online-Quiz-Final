using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Quiz_App.Data;
using Online_Quiz_App.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Quiz_App.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserProfileController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(int? id)
        {
            // If id is null, get from session (current user)
            int userId = id ?? int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var attempts = await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AttemptedAt)
                .ToListAsync();

            var model = new UserProfileViewModel
            {
                User = user,
                Attempts = attempts
            };
            return View(model);
        }
    }

    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<QuizAttempt> Attempts { get; set; }
    }
}
