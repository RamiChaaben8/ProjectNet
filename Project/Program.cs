using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Project.Data;
using Project.Repositories;
using System.Text;

namespace Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Disable file watching for uploads directory to prevent server restart
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB max request size
            });

            // Disable file watching in development mode for uploads directory
            builder.Host.ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    // This helps prevent reloads when files change in wwwroot/uploads
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                        optional: true, reloadOnChange: false);
                }
            });

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type=Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var cnx = builder.Configuration.GetConnectionString("cnx");
            builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(cnx)
            );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });

            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5276", "http://localhost:5053", "https://localhost:7053")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationContext>();
                
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            // Ensure wwwroot directory exists
            var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            // Ensure uploads directory exists
            var uploadsPath = Path.Combine(wwwrootPath, "uploads", "articles");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Create a custom file provider that excludes the uploads directory from watching
            var webRootFileProvider = new PhysicalFileProvider(wwwrootPath);
            
            // Enable static files with a non-watching file provider for uploads
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = webRootFileProvider,
                RequestPath = "",
                ServeUnknownFileTypes = false,
                OnPrepareResponse = ctx =>
                {
                    // Set cache headers for uploaded images
                    if (ctx.Context.Request.Path.StartsWithSegments("/uploads"))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=3600");
                    }
                }
            });

            app.UseCors("AllowBlazorClient");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}