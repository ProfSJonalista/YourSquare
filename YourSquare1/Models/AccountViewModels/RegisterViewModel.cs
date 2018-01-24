using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourSquare1.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Wypełnij to pole")]
        [EmailAddress(ErrorMessage = "To nie jest adres email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wypełnij to pole")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Wypełnij to pole")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Wypełnij to pole")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Wypełnij to pole")]
        [StringLength(100, ErrorMessage = "{0} musi mieć minimum {2} i maksymalnie {1} znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Hasła nie pasują do siebie")]
        public string ConfirmPassword { get; set; }
    }
}
