/**
 * This model contains all attributes to be displayed in the view for MRIViewModel
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain.View
{
    public class MRIViewModel
    {
        public string Id { get; set; }
        public string BookingId { get; set; }
        public string UserId { get; set; }
        public string StaffId { get; set; }
        public string UserName { get; set; }
        public string StaffName { get; set; }
        public string Description { get; set; }
        public string PostPath { get; set; }
        public int Status { get; set; }
        public DateTime BookingTime { get; set; }
        public DateTime MRITime { get; set; }

    }
}
