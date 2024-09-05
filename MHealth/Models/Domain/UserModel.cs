using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/**
 * This model contains all attributes to be used for User operation
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain
{
    public class UserModel : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
    }
}
