using INF27523_TP1_ML_SS.Data; 
using INF27523_TP1_ML_SS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Services
{
    public class AuthentificationService
    {
        private readonly ApplicationDbContext _context;

        public AuthentificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            return user;
        }
    }
}