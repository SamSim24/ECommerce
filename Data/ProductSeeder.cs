using INF27523_TP1_ML_SS.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace INF27523_TP1_ML_SS.Data
{
    public class ProductSeeder
    {
        //Méthode pour remplir la base de données avec les produits récupérés par l'API si cette dernière est vide
        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any()) 
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync("https://fakestoreapi.com/products");
                        if (response.IsSuccessStatusCode)
                        {
                            var products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(
                                await response.Content.ReadAsStringAsync(),
                                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (products != null && products.Count > 0)
                            {
                                foreach (var product in products)
                                {
                                    product.Id = 0; 
                                    product.SellerId = 10; 

                                    context.Products.Add(product);
                                }

                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors du chargement des produits : {ex.Message}");
                    }
                }
            }
        }
    }
}
