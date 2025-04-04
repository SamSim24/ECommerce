using INF27523_TP1_ML_SS.Data;
using INF27523_TP1_ML_SS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class PanierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PanierController> _logger;

        public PanierController(ApplicationDbContext context, ILogger<PanierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Retourne la vue du panier avec les items lié`s à l'utilisateur connecté
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentification");
            }
            var cartItems = _context.CartItems
                                    .Where(c => c.UserId == userId.Value)
                                    .ToList();
            ViewBag.Total = cartItems.Sum(item => item.Price * item.Quantity);
            return View(cartItems);
        }

        //Méthode pour rajouter un item dans le panier
        [HttpPost]
        public IActionResult AjouterAuPanier([FromBody] CartItem cartItem)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }
            var existingItem = _context.CartItems
                .FirstOrDefault(c => c.ProductId == cartItem.ProductId && c.UserId == userId.Value);

            //Si l'item existe déjà dans le panier, on incrémente le nombre de ce produit
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }

            //Sinon on crée un nouveau produit 
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = cartItem.ProductId,
                    UserId = userId.Value,
                    Title = cartItem.Title,
                    Price = cartItem.Price,
                    Image = cartItem.Image,
                    Quantity = 1
                };
                _context.CartItems.Add(newCartItem);
                _logger.LogInformation("Nouveau produit ajouté au panier");
            }
            _context.SaveChanges();
            return Json(new { message = "Produit ajouté au panier !" });
        }

        //Méthode pour le bouton "Poubelle" pour supprimer un produit du panier
        [HttpPost]
        public IActionResult ReduceQuantity(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.ProductId == productId && c.UserId == userId.Value);

            if (cartItem != null)
            {
                //Si plus que 1 produit, on décrémente le nombre du produit
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    _context.SaveChanges();

                    //Calculer le nouveau total pour ce produit
                    decimal newItemTotal = cartItem.Price * cartItem.Quantity;
                    
                    return Json(new
                    {
                        success = true,
                        message = "Quantité réduite",
                        newQuantity = cartItem.Quantity,
                        newItemTotal = newItemTotal
                    });
                }
                //Sinon, s'il y a qu'un produit, on le supprime de la bd
                else
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Produit supprimé",
                        removeItem = true
                    });
                }
            }

            return Json(new { success = false, message = "Produit non trouvé dans le panier." });
        }
    }
}