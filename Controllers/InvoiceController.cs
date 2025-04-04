using INF27523_TP1_ML_SS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Facture (Invoice)
        public async Task<IActionResult> Index()
        {
            // Verifier l'utilisateur
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentification");

            // Ont construit la facture avec
            var invoices = await _context.Invoices
                .Include(i => i.Order) // on recupere la commande associee
                    .ThenInclude(o => o.Items) // les items de la commandes
                    .ThenInclude(oi => oi.Product) // les produits de la commandes
                .Where(i => i.Order.UserId == userId.Value)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();

            return View(invoices);
        }

        // details de la facture 
        // /Invoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order) // inclure la commande
                    .ThenInclude(o => o.User) // associer a l'utilisateur
                .Include(i => i.Order)  // ajouter le produit et le vendeur de litem
                    .ThenInclude(o => o.Items) 
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Seller)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // Exporter en pdf
        public IActionResult ExportToPdf(int id)
        {
            var invoice = _context.Invoices // meme chose que details
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Seller)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            // Exporter en pdf grace a Rotativa
            return new ViewAsPdf("Pdf", invoice) // mais retourne vers la vue special pdf (sans boutton)
            {
                FileName = $"Facture_{invoice.Id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

    }
}
