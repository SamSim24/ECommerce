using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using INF27523_TP1_ML_SS.Models;
using INF27523_TP1_ML_SS.Data;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class ProfilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId"); //Récupère l'id du user connecté
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentification");
            }

            var user = await _context.Users   //Récupère le user avec l'id récupéré
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.EstVendeur)
            {
                var produits = await _context.Products
                    .Where(p => p.SellerId == userId)
                    .ToListAsync();

                var ventes = await _context.OrderItems
                    .Where(oi => oi.Product.SellerId == userId)
                    .SumAsync(oi => oi.Quantity * oi.UnitPrice);

                ViewBag.Produits = produits;
                ViewBag.TotalVentes = ventes;
            }

            return View(user);
        }

        //Méthode pour modifier le user
        [HttpPost]
        public async Task<IActionResult> ModifierProfil(User user)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentification");
            }

            if (userId.Value != user.Id)
            {
                return Unauthorized();
            }

            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                    }
                }

                return View("Index", user);
            }

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (existingUser == null)
                {
                    return NotFound();
                }

                // Mise à jour selective des propriétés
                existingUser.Name.Firstname = user.Name.Firstname;
                existingUser.Name.Lastname = user.Name.Lastname;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erreur lors de la modification du profil : " + ex.Message);
                return View("Index", user);
            }
        }

        //Méthode de déconnexion pour retourner à la page d'authentification
        public IActionResult Deconnexion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Authentification");
        }

    }
}