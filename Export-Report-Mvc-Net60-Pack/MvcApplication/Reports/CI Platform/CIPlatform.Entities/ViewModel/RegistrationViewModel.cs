using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatform.Entities.ViewModel
{
    public class RegistrationViewModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
                       ErrorMessage = "Password must be at least 8 characters long and contain at least one letter, one digit, and one special character.")]
        //[RegularExpression("^(?=.*[A-Za-z])(?=.*'\'d)(?=.*[@$!%*#?&])[A-Za-z'\'d@$!%*#?&]{8,}$")]
        [MinLength(8), MaxLength(20)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password")]
        public string CnfPassword { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[1-9][0-9]{9}")]
        //[MinLength(10,ErrorMessage ="enter 10 digit"), MaxLength(10, ErrorMessage = "enter 10 digit")]
        public long PhoneNumber { get; set; }

        // public string? redirecturlreturnUrl { get; set; }

    }
}
