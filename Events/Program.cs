using EventsApp.Data;
using EventsApp.Models;
using EventsApp.Repository.Interfaces;
using EventsApp.Repository.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Events
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


            // Get connection string from configuration.
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

            // Configure DbContext with SQL Server.
            builder.Services.AddDbContext<EventsAppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add Identity services with Entity Framework.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<EventsAppDbContext>();

            // Register application services.
            builder.Services.AddScoped<MailjetService>();
            builder.Services.AddScoped<IAccountx, IdentityAccountService>();

            // Configure Mailjet client.
            builder.Services.AddScoped(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                string apiKey = configuration["Mailjet:ApiKey"];
                string secretKey = configuration["Mailjet:SecretKey"];
                return new Mailjet.Client.MailjetClient(apiKey, secretKey);
            });






            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.Run();
        }
    }
}
