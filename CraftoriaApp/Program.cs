
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreDbContext>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IRawMaterialServices, RawMaterialServices>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();
            builder.Services.AddScoped<ITopRatedService, TopRatedService>();




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

            builder.Services.AddAutoMapper(M => M.AddProfile(new CartProfile()));
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
            });
            builder.Services.AddScoped<ITranslationService, TranslationService>();


            var cloudinaryUrl = builder.Configuration["Cloudinary:CloudinaryUrl"];

            if (string.IsNullOrWhiteSpace(cloudinaryUrl))
                throw new Exception("Cloudinary configuration is missing");

            Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
                await seeder.IdentityDataSeedingAsync();
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
