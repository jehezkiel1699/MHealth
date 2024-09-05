/**
 * This interface contains all services in Email
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Abstract
{
    public interface IEmailRepository
    {
        Task SendEmail(string fromEmail, string toEmail, string subject, string message, string attachmentPath = null);
    }
}
