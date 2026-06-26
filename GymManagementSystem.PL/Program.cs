using AutoMapper;
using GymManagementSystem.BLL;
using GymManagementSystem.BLL.Services.Attachment;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Data_Seeding;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)

    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllersWithViews();

        //builder.Services.AddScoped<IPlanRepository, PlanRepository>();

        //builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddScoped<ISessionRepository, SessionRepository>();

        builder.Services.AddScoped<IMemberService, MemberService>();

        builder.Services.AddScoped<IPlanService, PlanService>();

        builder.Services.AddScoped<ITrainerService, TrainerService>();

        builder.Services.AddScoped<ISessionService, SessionService>();

        builder.Services.AddScoped<IAnalyticService, AnalyticService>();

        builder.Services.AddScoped<IAttachmentService, AttachmentService>();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
        //builder.Services.AddTransient<IMapper, Mapper>();

        //builder.Services.AddScoped<GymDbContext>();

        //DI for GymDbContext with options - Scoped by default
        builder.Services.AddDbContext<GymDbContext>(options => 
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default Connection"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<GymDbContext>();


        var app = builder.Build();

        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GymDataSeeding>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
            await context.Database.MigrateAsync(); //Update-Database


        var planSeedFilePath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "files", "plans.json");
        await GymDataSeeding.SeedAsync<Plan>(context, planSeedFilePath, logger);

        await IdentityDataSeeding.SeedIdentityDataAsync(userManager, roleManager, logger);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();


        app.Run();

    }
}
