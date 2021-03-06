﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels.Account
{
    public class LoginVIewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage ="User Name cannot exceed characters")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
