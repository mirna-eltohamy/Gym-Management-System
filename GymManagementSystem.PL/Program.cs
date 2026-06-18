using AutoMapper;
using GymManagementSystem.BLL;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)

    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllersWithViews();

        //builder.Services.AddScoped<IPlanRepository, PlanRepository>();

        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddScoped<ISessionRepository, SessionRepository>();

        builder.Services.AddScoped<IMemberService, MemberService>();

        builder.Services.AddScoped<IPlanService, PlanService>();

        builder.Services.AddScoped<ITrainerService, TrainerService>();

        builder.Services.AddScoped<ISessionService, SessionService>();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
        //builder.Services.AddTransient<IMapper, Mapper>();

        //builder.Services.AddScoped<GymDbContext>();

        //DI for GymDbContext with options - Scoped by default
        builder.Services.AddDbContext<GymDbContext>(options => 
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default Connection"));
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
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();


        app.Run();

    }
}
