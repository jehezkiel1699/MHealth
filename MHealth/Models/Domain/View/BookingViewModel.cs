/**
 * This model contains all attributes to be displayed in the view for BookingView
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain.View
{
    public class BookingViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string StaffId { get; set; }
        public string MRIPostId { get; set; }
        public string UserName { get; set; }
        public string StaffName { get; set; }
        public string UserEmail { get; set; }
        public string StaffEmail { get; set; }
        public int Status { get; set; }
        public DateTime BookingTime { get; set; }

    }
}
