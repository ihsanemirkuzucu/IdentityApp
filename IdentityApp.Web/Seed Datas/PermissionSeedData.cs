using System.Security.Claims;
using IdentityApp.Web.Models;
using IdentityApp.Web.Permissions;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.Seed_Datas
{
    public class PermissionSeedData
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");
            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });
                var addedRole = await roleManager.FindByNameAsync("BasicRole");
                await AddReadPermission(addedRole, roleManager);
            }

            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });
                var addedRole = await roleManager.FindByNameAsync("AdvancedRole");
                await AddReadPermission(addedRole, roleManager);
                await AddUpdateandCreatePermission(addedRole, roleManager);
            }


            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });
                var addedRole = await roleManager.FindByNameAsync("AdminRole");
                await AddReadPermission(addedRole, roleManager);
                await AddUpdateandCreatePermission(addedRole, roleManager);
                await AddDeletePermission(addedRole, roleManager);
            }
        }

        public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Read));
        }

        public static async Task AddUpdateandCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Update));
        }

        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Delete));
        }

    }
}
