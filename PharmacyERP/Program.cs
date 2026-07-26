using Application.Common.Interfaces;
using FluentValidation;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PharmacyERP.Application.Common.Behaviors;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;
using PharmacyERP.Infrastructure.Repositories;
using PharmacyERP.Infrastructure.Services;
using QuestPDF.Infrastructure;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace PharmacyERP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("PharmacyERP"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(
                    name: "database",
                    tags: new[] { "db", "sql" });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status503ServiceUnavailable;

                options.AddConcurrencyLimiter("global", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.QueueLimit = 30;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddConcurrencyLimiter("heavy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.QueueLimit = 10;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy("register", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        "{\"message\":\"Too many requests, please try again shortly.\"}",
                        token);
                };
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddMediatR(typeof(RegisterHandler).Assembly);

            builder.Services.AddValidatorsFromAssemblies(
                AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IProductBatchRepository, ProductBatchRepository>();
            builder.Services.AddScoped<IProcurementRepository, ProcurementRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

            builder.Services.AddScoped<IExportService, ExportService>();
            builder.Services.AddScoped<IBarcodeService, BarcodeService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            builder.Services.AddSingleton<ICartService, CartService>();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                var userManager =
                    scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles =
{
    "Admin",
    "Pharmacist",
    "Cashier",
    "Customer",
    "PharmacyCompany"
};

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole<int>(role));
                }

                var adminEmail = "admin@pharmacy.com";
                var adminPassword = "Admin@123";

                var admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "System Admin",
                        PhoneNumber = "01110873153",
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
                else
                {
                    admin.FullName = "System Admin";
                    admin.PhoneNumber = "01110873153";
                    admin.IsActive = true;

                    await userManager.UpdateAsync(admin);

                    if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseRateLimiter();

                app.MapControllers()
                   .RequireRateLimiting("global");

                app.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status.ToString(),
                                description = e.Value.Description,
                                duration = e.Value.Duration.TotalMilliseconds
                            }),
                            totalDuration = report.TotalDuration.TotalMilliseconds
                        };

                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(response));
                    }
                });
                

                app.Run();
            }
        }
    }
}