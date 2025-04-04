using INF27523_TP1_ML_SS.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Data
{
    public class UserSeeder
    {
        //Méthode pour remplir la base de données avec les users récupérés par l'API si cette dernière est vide
        public static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any()) // Vérifie s'il y a déjà des utilisateurs
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync("https://fakestoreapi.com/users");
                        if (response.IsSuccessStatusCode)
                        {
                            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(
                                await response.Content.ReadAsStringAsync(),
                                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (users != null && users.Count > 0)
                            {
                                foreach (var user in users)
                                {
                                    user.Id = 0; // Pour éviter conflit avec EF
                                }

                                // Définir le dernier utilisateur comme vendeur
                                users[^1].EstVendeur = true; // ^1 = dernier élément

                                // Ajouter les utilisateurs après avoir défini tous les champs
                                context.Users.AddRange(users);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors du chargement des utilisateurs : {ex.Message}");
                    }
                }
            }
        }

    }
}

