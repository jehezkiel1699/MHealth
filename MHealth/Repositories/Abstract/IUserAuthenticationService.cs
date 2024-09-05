using MHealth.Models.DTO;

/**
 * This interface contains all services in User Authentication
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegistrationAsync(SignupModel model);

        Task<Status> GoogleCallback(string returnUrl, string remoteError);
        Task LogoutAsync();
    }
}
