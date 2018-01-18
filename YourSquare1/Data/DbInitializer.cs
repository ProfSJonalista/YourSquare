using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourSquare1.Models;

namespace YourSquare1.Data
{
    public class DbInitializer
    {
        internal static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Advertisments.Any())
            {
                return;
            }

            var TestAdverts = new Advertisment[]
            {
                new Advertisment(){ Description = "Test advert", UserID = "1" }
            };

            foreach (Advertisment a in TestAdverts)
            {
                context.Advertisments.Add(a);
            }

            context.SaveChanges();
        }
    }
}
