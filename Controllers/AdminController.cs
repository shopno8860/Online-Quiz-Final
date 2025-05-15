using Microsoft.AspNetCore.Mvc;
using Online_Quiz_App.Data;
using Online_Quiz_App.Models;
using System.Linq;

namespace Online_Quiz_App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context) => _context = context;

        // Admin Dashboard
        public IActionResult Index()
        {
            var teachers = _context.Users.Where(u => u.Role.ToLower() == "teacher").ToList();
            var students = _context.Users.Where(u => u.Role.ToLower() == "student").ToList();
            return View(new AdminDashboardViewModel { Teachers = teachers, Students = students });
        }

        // Add Teacher
        [HttpPost]
        public IActionResult AddTeacher(string fullName, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["Error"] = "Email already exists.";
                return RedirectToAction("Index");
            }
            _context.Users.Add(new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = "teacher",
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Add Student
        [HttpPost]
        public IActionResult AddStudent(string fullName, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["Error"] = "Email already exists.";
                return RedirectToAction("Index");
            }
            _context.Users.Add(new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = "student",
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Delete User
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Edit User (GET)
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Edit User (POST)
        [HttpPost]
        public IActionResult EditUser(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            // Only update password if not empty
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                user.Password = updatedUser.Password;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Redirect to Quiz Management
        public IActionResult ManageQuiz()
        {
            return RedirectToAction("Teacher", "Quiz");
        }
    }

    public class AdminDashboardViewModel
    {
        public List<User> Teachers { get; set; }
        public List<User> Students { get; set; }
    }
}
