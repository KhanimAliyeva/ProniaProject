using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.Account
{
    public class RegisterVM
    {
        [Required,MaxLength(32),MinLength(3)]
        public string LastName { get; set; }

        [Required, MaxLength(32), MinLength(3)]
        public string FirstName { get; set; }

        [Required, MaxLength(256), MinLength(3)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), MaxLength(256), MinLength(6)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password"), MaxLength(256), MinLength(6)]
        public string ConfirmPassword { get; set; }
    }
}
