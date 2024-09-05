/**
 * This model contains all attributes to be displayed in the view for UserPagination
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Models.Domain.View
{
    public class UserPaginationViewModel
    {
        public List<UserModel>? Users { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
