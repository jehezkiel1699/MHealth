using System.ComponentModel.DataAnnotations;

/**
 * This model contains all data in the database for SignUp
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.DTO
{
    public class SignupModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Name must be in string")]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Name must be in string")]
        public string? Username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$", ErrorMessage = "Password must consist of an uppercase letter and a number.")]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }
        public string? Role { get; set; }


    }
}
