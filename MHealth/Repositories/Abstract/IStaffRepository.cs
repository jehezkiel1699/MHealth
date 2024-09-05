using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;

/**
 * This interface contains all services in Staff
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Abstract
{
    public interface IStaffRepository
    {
        Task<IEnumerable<BookingViewModel>> GetAllBooking(string staffId);
        Task<UserModel> GetUserInformation(string userId);
        Task<MRIPostModel> WriteFile(IFormFile postImage);
        Task Post(MRIPostModel model);
        Task<BookingViewModel> GetBookingDetail(string bookingId);
    }
}
