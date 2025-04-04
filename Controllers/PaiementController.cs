using INF27523_TP1_ML_SS.Data;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using INF27523_TP1_ML_SS.Models;
using Microsoft.EntityFrameworkCore;
using InvoiceModel = INF27523_TP1_ML_SS.Models.Invoice;


namespace INF27523_TP1_ML_SS.Controllers
{
    public class PaiementController : Controller
    {
        // Initialisation
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public PaiementController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }


        // GET: /Paiement
        public IActionResult Index()
        {
            ViewBag.StripePublishableKey = _config["Stripe:PublishableKey"];
            return View();
        }


        // POST: /Paiement/CreateCheckoutSession
        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentification");
            }

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Votre panier est vide.";
                return RedirectToAction("Index", "Panier");
            }

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "cad",
                        UnitAmount = (long)(item.Price * 100), 
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title,
                            Images = new List<string> { item.Image }
                        }
                    }
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Paiement", null, Request.Scheme),
                CancelUrl = Url.Action("Index", "Paiement", null, Request.Scheme),
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Ancien code :
            // return Redirect(session.Url);

            // Nouveau pour intégration JS :
            return Json(new { id = session.Id });
        }

        // GET: /Paiement/Success
        public async Task<IActionResult> Success()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentification");
            }

            // Récupérer les articles du panier
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                ViewBag.Message = "Aucun article dans le panier.";
                return View();
            }

            // Créer la commande
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                Items = new List<OrderItem>()
            };

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                };
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Créer la facture
            var invoice = new InvoiceModel
            {
                OrderId = order.Id,
                InvoiceDate = DateTime.Now,
                Amount = order.TotalAmount
            };
            _context.Invoices.Add(invoice);

            // Supprimer le panier
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            ViewBag.Message = $"Paiement réussi. Facture #{invoice.Id} générée.";
            return View();
        }

    }
}