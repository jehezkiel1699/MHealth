/**
 * This model contains all data in the database for ErrorView
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}