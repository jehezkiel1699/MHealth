using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

/**
 * This model contains all data in the database for Login
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.DTO
{
    public class LoginModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Name must be in string")]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
