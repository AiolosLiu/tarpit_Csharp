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
                new AspNetUser(){ Username = "Administrator", Password = "PowerfullBoss"},
                new AspNetUser(){ Username = "meredith12", Password = "meredith12"},
                new AspNetUser(){ Username = "Arturo", Password = "Arturo"},
                new AspNetUser(){ Username = "Gytis", Password = "Gytis"},
                new AspNetUser(){ Username = "Yan", Password = "Yan"},
                new AspNetUser(){ Username = "Peggy", Password = "Peggy"},
                new AspNetUser(){ Username = "Laura", Password = "Laura"},
                new AspNetUser(){ Username = "Nino", Password = "Nino"},
            };


            foreach (var s in users)
            {
                await context.Users.AddAsync(s);
            }
            await context.SaveChangesAsync();

            var students = new Order[]
            {
                new Order { FirstName = "Carson",   LastName = "Alexander",
                    AspNetUserId = users.Single(s => s.Username == "Administrator").Id,
                     },
                new Order { FirstName = "Meredith", LastName = "Alonso",
                    AspNetUserId = users.Single(s => s.Username == "meredith12").Id,
                    },
                new Order { FirstName = "Arturo",   LastName = "Anand",
                    AspNetUserId = users.Single(s => s.Username == "Arturo").Id,
                    },
                new Order { FirstName = "Gytis",    LastName = "Barzdukas",
                    AspNetUserId = users.Single(s => s.Username == "Gytis").Id,
                    },
                new Order { FirstName = "Yan",      LastName = "Li",
                    AspNetUserId = users.Single(s => s.Username == "Yan").Id,
                    },
                new Order { FirstName = "Peggy",    LastName = "Justice",
                    AspNetUserId = users.Single(s => s.Username == "Peggy").Id,
                    },
                new Order { FirstName = "Laura",    LastName = "Norman",
                    AspNetUserId = users.Single(s => s.Username == "Laura").Id,
                    },
                new Order { FirstName = "Nino",     LastName = "Olivetto",
                    AspNetUserId = users.Single(s => s.Username == "Nino").Id,
                    }
            };

            foreach (Order s in students)
            {
                await context.Orders.AddAsync(s);
            }
            await context.SaveChangesAsync();


            var courses = new Course[]
            {
                new Course {Title = "Chemistry",      Credits = 3,
                },
                new Course {Title = "Microeconomics", Credits = 3,
                },
                new Course {Title = "Macroeconomics", Credits = 3,
                },
                new Course {Title = "Calculus",       Credits = 4,
                },
                new Course {Title = "Trigonometry",   Credits = 4,
                },
                new Course {Title = "Composition",    Credits = 3,
                },
                new Course {Title = "Literature",     Credits = 4,
                },
            };

            foreach (Course c in courses)
            {
                await context.Courses.AddAsync(c);
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