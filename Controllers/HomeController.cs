using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INF27523_TP1_ML_SS.Models;
using INF27523_TP1_ML_SS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; 

namespace INF27523_TP1_ML_SS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Afficher la vue avec les produits dans Home selon les filtres et recherche
        public async Task<IActionResult> Index(string category = "", string search = "")
        {
            var productsQuery = _context.Products.Include(p => p.Rating).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Title.Contains(search));
            }

            var products = await productsQuery.ToListAsync();

            var categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;

            return View(products);
        }

        //Afficher la page des Details des produits
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Rating)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //Afficher la page d'erreur si mauvaise requête
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
