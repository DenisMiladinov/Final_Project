using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using Services.Services;

namespace Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin",
                    policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireReceptionist",
                    policy => policy.RequireRole("Receptionist"));
            });


            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IVacationSpotRepository, VacationSpotRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();

            builder.Services.AddScoped<IVacationSpotService, VacationSpotService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            var stripeKey = builder.Configuration["Stripe:SecretKey"];
            Stripe.StripeConfiguration.ApiKey = stripeKey;

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userStore = services.GetRequiredService<IUserStore<ApplicationUser>>();

                await DataSeeder.SeedAsync(dbContext, userManager, roleManager, userStore);
            }

            app.Run();
        }
    }
}
   