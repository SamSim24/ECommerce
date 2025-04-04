using INF27523_TP1_ML_SS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Controllers
{
    public class AuthentificationController : Controller
    {
        private readonly AuthentificationService _authentificationService;

        public AuthentificationController(AuthentificationService authentificationService)
        {
            _authentificationService = authentificationService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _authentificationService.AuthenticateAsync(username, password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Identifiants incorrects";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}