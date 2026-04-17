
using AutoMapper;
using CloudinaryDotNet;
using CraftoriaApp.CustomeMiddleWares;
using CraftoriaApp.Validators;
using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data.Contexts;
using Persistance.Repositories;
using Service;
using Service.Factory;
using Service.Mapping_Profiles;
using Service.MappingProfiles;
using ServiceAbstraction;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CraftoriaApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Craftoria API",
                    Version = "v1"
                });

                // 1. إضافة تعريف الـ Security (بنعرف Swagger إن فيه حاجة اسمها Bearer Token)
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token in this format: Bearer {your_token_here}"
                });

                // 2. تفعيل الـ Security Requirement (عشان يربط الـ Token بكل الطلبات)
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
            builder.Services.AddDbContext<StoreDbContext>(options =>

            {
                options.UseSqlServer(

                    builder.Configuration.GetConnectionString("Connection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    )
                );
            });

            //builder.Services.AddDbContext<StoreDbContext>(options =>
            //{
            //    options.UseNpgsql(
            //        builder.Configuration.GetConnectionString("PostgresConnection"),
            //        npgsqlOptions =>
            //        {
            //            npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);

            //            npgsqlOptions.MigrationsHistoryTable("__PostgresMigrationHistory");
            //        }
            //    );
            //});
            // 1. قبل builder.Build()
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin() // بيسمح لأي مكان يكلم السيرفر
                               .AllowAnyMethod() // بيسمح بكل العمليات (GET, POST, etc)
                               .AllowAnyHeader(); // بيسمح بكل الهيدرز بما فيها الـ Authorization
                    });
            });
            builder.Services.AddHttpClient<IChatBotService, ChatBotService>(client =>
            {
                client.BaseAddress = new Uri("https://ml-api-727549809675.me-central1.run.app/");

         
                client.Timeout = TimeSpan.FromMinutes(3);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // تسجيل الـ Service نفسها كـ Scoped
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreDbContext>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IRawMaterialServices, RawMaterialServices>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();
            builder.Services.AddScoped<ITopRatedService, TopRatedService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<PaymobService>();
            builder.Services.AddScoped<StripeService>();
            builder.Services.AddScoped<PaymentServiceFactory>();



            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped<ICategoriesSeeding, CategoriesSeeding>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IUserInteractionRepository, UserInteractionRepository>();
            builder.Services.AddScoped<IUserInteractionService, UserInteractionService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICartRepository, CartRpository>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new CartProfile()));
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
           

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("redisConnection")!);
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               
            .AddJwtBearer(options =>
            {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,

                   ValidIssuer = builder.Configuration["JWTOptions:issuer"],
                   ValidAudience = builder.Configuration["JWTOptions:audience"],

                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8!.GetBytes(builder.Configuration["JWTOptions:secretKey"]))
               };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // السطر ده هيطبع لك السبب الحقيقي في الـ Output بتاع Visual Studio
                        Console.WriteLine("❌ Token failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validated successfully!");
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddScoped<ITranslationService, TranslationService>();

            var cloudinaryUrl = builder.Configuration["Cloudinary:CloudinaryUrl"];

            if (string.IsNullOrWhiteSpace(cloudinaryUrl))
                throw new Exception("Cloudinary configuration is missing");

            Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);
            var app = builder.Build();
            app.UseCors("AllowAll"); // 👈 لازم السطر ده يكون قبل UseAuthentication و UseAuthorization
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            //    try
            //    {
            //        var context = services.GetRequiredService<StoreDbContext>();

            //        // 1️⃣ الخطوة الأولى: إنشاء الجداول فوراً (لو مش موجودة)
            //        // لازم دي تكون أول خطوة قبل أي عملية Seeding
            //        await context.Database.EnsureCreatedAsync();
            //        Console.WriteLine("✅ Database structure is ready (EnsureCreated).");

            //        // 2️⃣ الخطوة الثانية: ملء بيانات الـ Identity (Users, Roles)
            //        var seeder = services.GetRequiredService<IDataSeeding>();
            //        await seeder.IdentityDataSeedingAsync();
            //        await seeder.SeedCustomersDataAsync();

            //        // 3️⃣ الخطوة الثالثة: ملء بيانات الـ Categories والمنتجات
            //        var catSeeder = services.GetRequiredService<ICategoriesSeeding>();
            //        await catSeeder.ProductCategoryDataSeedingAsync();
            //        await catSeeder.RawMaterialsCategoryDataSeedingAsync();

            //        Console.WriteLine("✅ All Data Seeding completed successfully!");
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = loggerFactory.CreateLogger<Program>();
            //        logger.LogError(ex, "❌ An error occurred during database setup or seeding.");
            //    }
            //}

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
                await seeder.IdentityDataSeedingAsync();
                //await seeder.SeedCustomersDataAsync();
            }
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ICategoriesSeeding>();
                await seeder.ProductCategoryDataSeedingAsync();
                await seeder.RawMaterialsCategoryDataSeedingAsync();
            }
            app.UseMiddleware<CustomeExceptionHandlerMiddleWare>();
            var supportedCultures = new[] { "en", "ar" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions); // ???? ????? ?? ???? ??? app.UseAuthorization
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {

                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });

            }
                app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
