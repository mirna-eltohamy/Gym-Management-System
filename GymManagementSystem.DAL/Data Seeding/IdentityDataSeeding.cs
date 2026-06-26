using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data_Seeding
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(
             UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager,
             ILogger logger,
             CancellationToken ct = default)
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync();
                var hasRoles = await roleManager.Roles.AnyAsync();

                if (hasUsers && hasRoles) return;
                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>()
                {
                    new IdentityRole(roleName: "SuperAdmin"),
                    new IdentityRole(roleName: "Admin"),
                };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            await roleManager.CreateAsync(role);
                        }
                    }

                }

                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName = "Khaled",
                        UserName = "ahmedkhaled",
                        Email = "ahmedkhaled12@gmail.com",
                        PhoneNumber = "011111111111"
                    };

                    await userManager.CreateAsync(superAdmin, password: "P@ssw0rd");
                    await userManager.AddToRoleAsync(superAdmin, role: "SuperAdmin");
                    var admin = new ApplicationUser()
                    {
                        FirstName = "mohamed",
                        LastName = "gamal",
                        UserName = "mohamedgamal",
                        Email = "mohamedgamal@gmail.com",
                        PhoneNumber = "011111111111"
                    };

                    await userManager.CreateAsync(admin, password: "P@ssw0rd");
                    await userManager.AddToRoleAsync(admin, role: "Admin");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return;
            }

        }
    }
}
