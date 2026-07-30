using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.DTO.UserDTO;
using Application.Interfaces;
using Application.Result;
using Application.Services;
using Domain.Users;
using dotenv.net;
using Infrastructure.AppDbContexts;
using Presentation.ExceptionMiddlewares;

// microsoft
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            // amazon s3 client
            string? GARAGE_API_KEY = Environment.GetEnvironmentVariable("GARAGE_API_KEY");
            string? GARAGE_SECRET_KEY = Environment.GetEnvironmentVariable("GARAGE_SECRET_KEY");
            string? GARAGE_REGION = Environment.GetEnvironmentVariable("GARAGE_REGION");
            string? SERVICE_URL = Environment.GetEnvironmentVariable("GARAGE_SERVICE_URL");

            if (GARAGE_API_KEY == null || GARAGE_SECRET_KEY == null || GARAGE_REGION == null || SERVICE_URL == null)
            {
                Console.WriteLine("Проверьте данные Garage");
                return;
            }

            var credentials = new BasicAWSCredentials(GARAGE_API_KEY, GARAGE_SECRET_KEY);
            var region = RegionEndpoint.GetBySystemName(GARAGE_REGION);
            var forcePathStyle = builder.Configuration.GetValue<bool>("S3Config:ForcePathStyle");
            

            var s3Config = new AmazonS3Config
            {
                ServiceURL = SERVICE_URL,
                ForcePathStyle = forcePathStyle
            };

            builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, s3Config)); // adding S3Client
            // amazon s3 client
            
            // services;
            builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {          
                app.UseExceptionHandler("/Error");   
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            }); 

            app.UseHttpsRedirection();
            app.UseRouting();

            // middleware
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            // middleware

            app.MapControllers();

            app.Run();
        }
    }
}