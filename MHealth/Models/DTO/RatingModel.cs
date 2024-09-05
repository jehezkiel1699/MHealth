using System.ComponentModel.DataAnnotations;

/**
 * This model contains all data in the database for Rating
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.DTO
{
    public class RatingModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string StaffId { get; set; }
        [Required]
        public int Rating { get; set; }
    }
}
