using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourSquare1.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "Coś tam {0} musi być {2} blabla {1} lol.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Kod autoryzacji")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Pamiętaj mnie")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
