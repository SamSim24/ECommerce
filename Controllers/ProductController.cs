using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using INF27523_TP1_ML_SS.Data;
using INF27523_TP1_ML_SS.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index(string category, string search)
        {
            var products = _context.Products.Include(p => p.Seller).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Title.Contains(search));
            }

            ViewBag.Categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            return View(await products.ToListAsync());
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Seller)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != 10) return Unauthorized();
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != 10) return Unauthorized();

            if (ModelState.IsValid)
            {
                product.SellerId = userId.Value;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(product);
        }

      
    }
}
