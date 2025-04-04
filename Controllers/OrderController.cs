using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using INF27523_TP1_ML_SS.Data;
using Rotativa.AspNetCore;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() // Ramene a la vue Index qui permet a l'utilisateur
        {
            // Verification de la connexion a un compte valide
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            // Commande
            var orders = await _context.Orders
                .Include(o => o.Items) // Ajouter l'item
                    .ThenInclude(i => i.Product) // Ajouter le produit
                        .ThenInclude(p => p.Seller) // Ajouter le vendeur
                .Where(o => o.UserId == userId) // a l'utilisateur actuel
                .OrderByDescending(o => o.OrderDate) // et ont classe
                .ToListAsync(); // dans une liste

            return View(orders); // Ont retourne la vue Index avec la commande (orders)
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Seller) 
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        public IActionResult ExportToPdf(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            // Verifie si est vendeur, la commande ne peut etre imprime que par les vendeurs
            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null || !user.EstVendeur)
                return Unauthorized();

            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Seller)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return new ViewAsPdf("Pdf", order)
            {
                FileName = $"Commande_{order.Id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
        public async Task<IActionResult> SalesHistory()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null || !user.EstVendeur)
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.SellerId == userId.Value))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.VendorId = userId.Value;

            return View("Historique", orders);
        }


    }
}
