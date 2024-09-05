using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;

/**
 * This interface contains all services in Admin
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Abstract
{
    public interface IAdminRepository
    {
        Task<IEnumerable<UserModel>> GetAllUser();
        Task<IEnumerable<BookingViewModel>> GetAllBooking();
        Task<UserModel> GetUserById(string id);
        Task<BookingModel> GetBookingById(string id);
        Task<Status> UpdateUser(UserViewModel model);
        Task<Status> DeleteUser(string id);
        Task<Status> DeleteBooking(string id);
        Task<Status> CreateStaff(SignupModel model);
        Task<UserViewModel> UserModelToUserViewModel(string id);
        Task<IEnumerable<UserModel>> SelectUser(BulkEmailViewModel model);
        Task<IEnumerable<RatingViewModel>> GetAverageRating();

    }
}
