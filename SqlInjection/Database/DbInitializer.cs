using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SqlInjection.Models;

namespace SqlInjection.Database
{
    public static class DbInitializer
    {
        public static async Task Initialize(SchoolContext context)
        {

            await WaitForDb(context);

            context.Database.Migrate();
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }

            var users = new AspNetUser[]
            {
                new AspNetUser(){ Username = "Admin", Password = "*_admin"},
                new AspNetUser(){ Username = "Temp", Password = "!_temp"},
                new AspNetUser(){ Username = "Guest", Password = "@_guest"},
                new AspNetUser(){ Username = "Guest2", Password = "@_guest2"},
            };


            foreach (var s in users)
            {
                await context.Users.AddAsync(s);
            }
            await context.SaveChangesAsync();

            await context.Database.ExecuteSqlCommandAsync("DELETE FROM Student WHERE 1=1;");
            var orders = new Order[]
            {
                new Order { FirstName = "Cooper",   LastName = "Sheldon",   //Id = 0,
                    AspNetUserId = users.Single(s => s.Username == "Admin").Id,
                     },
                new Order { FirstName = "Hofstadter", LastName = "Leonard", //Id = 1,
                    AspNetUserId = users.Single(s => s.Username == "Temp").Id,
                    },
                new Order { FirstName = "Wolowitz",   LastName = "Howard",  //Id = 2,
                    AspNetUserId = users.Single(s => s.Username == "Guest").Id,
                    },
                new Order { FirstName = "Koothrappali",  LastName = "Raj",  //Id = 3,
                    AspNetUserId = users.Single(s => s.Username == "Guest2").Id,
                    },
            };

            foreach (Order s in orders)
            {
                await context.Orders.AddAsync(s);
            }
            await context.SaveChangesAsync();

        }


        private static async Task WaitForDb(DbContext context)
        {
            var maxAttemps = 12;
            var delay = 5000;

            var healthChecker = new DbHealthChecker();
            for (int i = 0; i < maxAttemps; i++)
            {
                if (healthChecker.TestConnection(context))
                {
                    return;
                }
                await Task.Delay(delay);
            }

            // after a few attemps we give up
            throw new Exception("Error wating database");

        }
    }
}