using System.ComponentModel.DataAnnotations;

namespace MHealth.Models.Domain
{
    public class AnnouncementModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Announcement { get; set; }
    }
}
