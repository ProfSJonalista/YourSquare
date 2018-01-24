using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourSquare1.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Niepoprawny email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Niepoprawne hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
