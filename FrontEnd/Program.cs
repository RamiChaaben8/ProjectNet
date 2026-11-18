using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FrontEnd
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Register services
            builder.Services.AddScoped<Services.IAuthService, Services.AuthService>();
            builder.Services.AddScoped<Services.IUserService, Services.UserService>();
            builder.Services.AddScoped<Services.IImageUploadService, Services.ImageUploadService>();
            
            builder.Services.AddScoped<Services.CategoryService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var authService = sp.GetRequiredService<Services.IAuthService>();
                return new Services.CategoryService(httpClient, authService);
            });
            builder.Services.AddScoped<Services.ArticleService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var authService = sp.GetRequiredService<Services.IAuthService>();
                return new Services.ArticleService(httpClient, authService);
            });
            builder.Services.AddScoped<Services.CartService>();
            builder.Services.AddScoped<Services.OrderService>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                var authService = sp.GetRequiredService<Services.IAuthService>();
                return new Services.OrderService(httpClient, authService);
            });
            
            

            // Simple HttpClient without message handler
            builder.Services.AddScoped(sp =>
            {
                var cofiguration = sp.GetRequiredService<IConfiguration>();
                var backendUrl = cofiguration["BackendUrl"];
                if (string.IsNullOrEmpty(backendUrl))
                {
                    throw new InvalidOperationException("BackendUrl is not configured.");
                }
                return new HttpClient { BaseAddress = new Uri(backendUrl) };
            });

            await builder.Build().RunAsync();
        }
    }
}
