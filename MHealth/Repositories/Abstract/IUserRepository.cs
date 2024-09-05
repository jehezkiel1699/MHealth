using MHealth.Models.Domain;

/**
 * This interface contains all services in User
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetAllStaff();
    }
}
