using INF27523_TP1_ML_SS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class VendeurController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendeurController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Profil()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            // Produits du vendeur
            var produits = await _context.Products
                .Where(p => p.SellerId == userId)
                .ToListAsync();

            // Total des ventes (revenus) : somme des produits vendus * quantité
            var revenus = await _context.OrderItems
                .Where(oi => oi.Product.SellerId == userId)
                .SumAsync(oi => oi.UnitPrice * oi.Quantity);

            ViewBag.RevenusTotaux = revenus;
            return View(produits);
        }
        public async Task<IActionResult> Historique()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            var orders = await _context.Orders
                .Include(o => o.User) // Inclut les informations sur le client
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product) // Inclut les produits commandés
                .Where(o => o.Items.Any(i => i.Product.SellerId == userId)) // Filtrer par vendeur
                .ToListAsync();

            ViewBag.VendorId = userId.Value;
            return View(orders);
        }

    }
}
