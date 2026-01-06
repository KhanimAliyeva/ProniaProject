using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.Account
{
    public class LoginVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), MaxLength(256), MinLength(6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
