using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace YourSquare1.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool Administrator { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public ICollection<Advertisment> Advertisments { get; set; }
    }
}
