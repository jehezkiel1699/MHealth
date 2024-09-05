using System.ComponentModel.DataAnnotations;

/**
 * This model contains all attributes to be displayed in the view for BulkEmailView
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain.View
{
    public class BulkEmailViewModel
    {
        //public List<UserModel> Users { get; set; }
        [Required]
        public List<string> SelectedUserIds { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Announcement { get; set; }
    }
}
