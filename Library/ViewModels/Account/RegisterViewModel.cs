using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "User Name must be between 5 and 50 characters", MinimumLength = 5)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password must be between 6 and 50 characters", MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
