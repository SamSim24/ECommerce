using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using INF27523_TP1_ML_SS.Data;
using INF27523_TP1_ML_SS.Services;
using Microsoft.AspNetCore.Http;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<AuthentificationService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate(); // Appliquer les migrations
        await UserSeeder.SeedUsersAsync(context); // Remplir la base de données des users de l'API
        await ProductSeeder.SeedProductsAsync(context); // Remplir la base de données des produits de l'API
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de l'application des migrations : {ex.Message}");
    }
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "paiement_test",
    pattern: "paiement-test",
    defaults: new { controller = "Paiement", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentification}/{action=Login}/{id?}");
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");
app.Run();