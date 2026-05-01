
using AutoMapper;
using CloudinaryDotNet;
using CraftoriaApp.CustomeMiddleWares;
using CraftoriaApp.Helper;
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
using Presentation.Hubs;
using Presentation.Services;
using Service;
using Service.Factory;
using Service.Mapping_Profiles;
using Service.MappingProfiles;
using Service.Use_Case;
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

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    // التغيير هنا: نوعه Http عشان Swagger يفهم إنه Bearer Scheme
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Put **ONLY** your JWT token below. Swagger will add 'Bearer ' for you."
                });
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
                options.OperationFilter<DefaultResponsesOperationFilter>();
            });
            builder.Services.AddDbContext<StoreDbContext>(options =>

            {
                options.UseSqlServer(

                    builder.Configuration.GetConnectionString("localConnection"),
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


            // ── CORS ──────────────────────────────────────────────────────────────
            // NOTE FOR PRODUCTION: Replace AllowAnyOrigin with your specific origins
            // AND add .AllowCredentials() if you use long-polling SignalR transport.
            // For WebSocket + JWT-in-query-string (recommended), AllowAnyOrigin is fine.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.SetIsOriginAllowed(_ => true) // بيسمح لأي مكان يكلم السيرفر
                               .AllowAnyMethod() // بيسمح بكل العمليات (GET, POST, etc)
                               .AllowAnyHeader() // بيسمح بكل الهيدرز بما فيها الـ Authorization
                               .AllowCredentials(); // لو بتستخدم WebSockets مع JWT في الـ query string، لازم تسمح بالكريدنشالز
                    });
            });

            // ── SignalR ───────────────────────────────────────────────────────────
            // AddSignalR() registers the hub infrastructure.
            // The hub itself is mapped to a route further below (app.MapHub).
            builder.Services.AddSignalR(options =>
            {
                // Increase timeout for mobile clients on poor connections
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            });

            // ── HTTP Clients ──────────────────────────────────────────────────────

            builder.Services.AddHttpClient<IChatBotService, ChatBotService>(client =>
            {
                client.BaseAddress = new Uri("https://ml-api-727549809675.me-central1.run.app/");


                client.Timeout = TimeSpan.FromMinutes(3);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            // ── Identity ──────────────────────────────────────────────────────────
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
            // سجل الـ Repository الخاص بالبوستات
            builder.Services.AddScoped<IPostRepository, PostRepository>();

            // سجل الـ PostService اللي شايل كل الـ Logic
            builder.Services.AddScoped<IPostService, PostService>(); builder.Services.AddHttpClient<IRecommendationService, RecommendationService>(client =>
            {
                // بنجيب السكشن كامل
                var mlSettings = builder.Configuration.GetSection("MLApiSettings");
                var baseUrl = mlSettings["BaseUrl"];

                if (string.IsNullOrEmpty(baseUrl))
                {
                    // ده هيظهر لك في الـ Console وأنتِ بتشغلي الـ App عشان تعرفي المشكلة فين بالظبط
                    Console.WriteLine("CRITICAL: MLApiSettings:BaseUrl is null. Check appsettings.json format.");
                    baseUrl = "https://ml-api-727549809675.me-central1.run.app/"; // fallback للامان
                }
                


                client.BaseAddress = new Uri(baseUrl);
            });
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

            // ── NEW: Chat / Messaging Services ────────────────────────────────────

            // The BRIDGE: Service layer calls IRealtimeNotificationService,
            // Presentation layer provides the SignalR implementation.
            // This resolves the circular dependency without breaking Clean Architecture.
            builder.Services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();

            // The chat business-logic service
            builder.Services.AddScoped<IMessageService, MessageService>();


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
                    },

                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // Only apply to SignalR hub route
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            builder.Services.AddScoped<ICartRepository, CartRpository>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new CartProfile()));
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();
            builder.Services.AddScoped<IGetRecommendedProductsUseCase, GetRecommendedProductsUseCase>();
            builder.Services.AddScoped<ITranslationService, TranslationService>();

            var cloudinaryUrl = builder.Configuration["Cloudinary:CloudinaryUrl"];

            if (string.IsNullOrWhiteSpace(cloudinaryUrl))
                throw new Exception("Cloudinary configuration is missing");

            Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);
            var app = builder.Build();
            app.UseCors("AllowAll"); // 👈 لازم السطر ده يكون قبل UseAuthentication و UseAuthorization
            

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

            app.UseRouting();          // أول حاجة
            app.UseCors("AllowAll");   // بعد UseRouting مباشرة
           // app.UseHttpsRedirection(); ← علّق السطر ده
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<ChatHub>("/hubs/chat").RequireAuthorization();
            app.Run();
            //app.UseHttpsRedirection();

            //app.UseAuthentication();
            //app.UseAuthorization();


            //app.MapControllers();

            //// ── SignalR Hub Route ─────────────────────────────────────────────────
            //// Clients connect to: wss://yourserver/hubs/chat?access_token=<jwt>
            //// The hub requires [Authorize] so unauthenticated connections are rejected.
            //app.MapHub<ChatHub>("/hubs/chat")
            //    .RequireAuthorization();   // belt-and-suspenders on top of [Authorize]

            //app.Run();
        }
    }
}
