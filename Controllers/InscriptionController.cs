using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using INF27523_TP1_ML_SS.Data;
using INF27523_TP1_ML_SS.Models;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class InscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Inscription()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Inscription(string firstname, string lastname, string email, string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Les mots de passe ne correspondent pas.";
                return View();
            }

            //Vérifier si l'utilisateur existe déjà
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
            {
                ViewBag.Error = "Ce nom d'utilisateur est déjà pris.";
                return View();
            }

            //Création du nouveau user
            var newUser = new User
            {
                Name = new User.NameModel { Firstname = firstname, Lastname = lastname },
                Email = email,
                Username = username,
                Password = password 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Connexion automatique après l'inscription
            HttpContext.Session.SetInt32("UserId", newUser.Id);

            return RedirectToAction("Index", "Home");
        }
    }
}
