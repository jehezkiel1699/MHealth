using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/**
 * This model contains all attributes to be displayed in the view for UserView
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain
{
    //[NotMapped]
    public class UserViewModel 
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }
    }
}
