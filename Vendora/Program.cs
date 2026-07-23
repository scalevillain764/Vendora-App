using Application.Interfaces;
using Application.Services;
using Domain.Users;
using dotenv.net;
using Infrastructure.AppDbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// microsoft
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Vendora
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;

                    var sym_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY")));
                    options.TokenValidationParameters.IssuerSigningKey = sym_key;

                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                }
            );

            builder.Services.AddAuthorization();

            // логгер
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();


            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
                return;

            builder.Services.AddDbContext<AppDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // middleware
           /* app.UseMiddleware<ExceptionMiddleware>();*/
            app.UseAuthentication();
            app.UseAuthorization();
            // middleware

            app.MapControllers();

            app.Run();
        }
    }
}

// сделать комит: изменил app db context, orderItem, product. ругается на ulid