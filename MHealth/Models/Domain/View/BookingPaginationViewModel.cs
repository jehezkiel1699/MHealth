/**
 * This model contains all attributes to be displayed in the view for BookingPagination
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain.View
{
    public class BookingPaginationViewModel
    {
        public List<BookingViewModel> Bookings { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
