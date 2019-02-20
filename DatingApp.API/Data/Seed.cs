using System.Collections.Generic;
using System.IO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> userManager;

        public Seed(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public void SeedUsers()
        {
            if (userManager.Users.Any()) return;

            var userData = File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in users) userManager.CreateAsync(user, "password").Wait();
        }
    }
}