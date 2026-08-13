using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.DTO.UserDTO;
using Application.Interfaces;
using Application.Result;
using Application.Services;
using Domain.Users;
using dotenv.net;
using FluentValidation;
using FluentValidation.Validators;
using Infrastructure.AppDbContexts;

// microsoft
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Presentation.ExceptionMiddlewares;
using SharpGrip.FluentValidation.AutoValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
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

            // validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            // amazon s3 client
            string? GARAGE_API_KEY = Environment.GetEnvironmentVariable("GARAGE_API_KEY");
            string? GARAGE_SECRET_KEY = Environment.GetEnvironmentVariable("GARAGE_SECRET_KEY");
            string? GARAGE_REGION = Environment.GetEnvironmentVariable("GARAGE_REGION");
            string? SERVICE_URL = Environment.GetEnvironmentVariable("GARAGE_SERVICE_URL");

            if (string.IsNullOrEmpty(GARAGE_API_KEY) ||
                string.IsNullOrEmpty(GARAGE_SECRET_KEY) ||
                string.IsNullOrEmpty(GARAGE_REGION) ||
                string.IsNullOrEmpty(SERVICE_URL))
            {
                throw new InvalidOperationException(
                    $"Ошибки конфигурации Garage:\n" +
                    $"API_KEY: '{GARAGE_API_KEY}'\n" +
                    $"SECRET_KEY: '{GARAGE_SECRET_KEY}'\n" +
                    $"REGION: '{GARAGE_REGION}'\n" +
                    $"SERVICE_URL: '{SERVICE_URL}'"
                );
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
            builder.Services.AddScoped<IS3Service, S3Service>();
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
            builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
            builder.Services.AddScoped<IUserQuestionService, UserQuestionService>();
            builder.Services.AddScoped<IProductForStoreStatisticsService, ProductForStoreStatisticsService>();

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

            // postgre
            string? connectionString = null;
            string? connectionPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            string? connectionDatabase = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
            string? connectionUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
            string? connectionPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

            if (string.IsNullOrEmpty(connectionPort) ||
               string.IsNullOrEmpty(connectionDatabase) ||
               string.IsNullOrEmpty(connectionUsername) ||
               string.IsNullOrEmpty(connectionPassword))
                throw new InvalidOperationException(
                    $"Configuration error in PostgreSQL:\n" +
                    $"PORT: '{connectionPort ?? "Undefined"}'\n" +
                    $"DATABASE: '{connectionDatabase ?? "Undefined"}'\n" +
                    $"USERNAME: '{connectionUsername ?? "Undefined"}'\n" +
                    $"PASSWORD: '{connectionPassword ?? "Undefined"}'"
                );

            connectionString =
                $"Host=localhost;Port={connectionPort};Database={connectionDatabase};Username={connectionUsername};Password={connectionPassword}";

            builder.Services.AddDbContext<AppDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            /* builder.Services.AddSwaggerGen();*/
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] =
                            new List<string>()
                    });
            });

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

