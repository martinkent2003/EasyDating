using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Controllers;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context){
            if (await context.Users.AnyAsync()) return;
            var userData = await File.ReadAllTextAsync("Data/UserDataSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}