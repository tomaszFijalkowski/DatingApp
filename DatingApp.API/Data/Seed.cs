using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void SeedUsers()
        {
            if (EnumerableExtensions.Any(userManager.Users)) return;

            var userData = File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            var roles = new List<Role>
            {
                new Role {Name = "Member"},
                new Role {Name = "Admin"},
                new Role {Name = "Moderator"},
                new Role {Name = "VIP"}
            };

            foreach (var role in roles)
            {
                roleManager.CreateAsync(role).Wait();
            }

            foreach (var user in users)
            {
                user.Photos.SingleOrDefault().IsApproved = true;
                userManager.CreateAsync(user, "password").Wait();
                userManager.AddToRoleAsync(user, "Member").Wait();
            }

            var adminUser = new User
            {
                UserName = "Admin"
            };

            IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

            if (result.Succeeded)
            {
                var admin = userManager.FindByNameAsync("Admin").Result;
                userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}).Wait();
            }
        }
    }
}