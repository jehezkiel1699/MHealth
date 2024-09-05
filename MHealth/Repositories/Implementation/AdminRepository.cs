using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/**
 * This Repository stores all admin services
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Implementation
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<UserModel> _userManager;

        /**
        * Default constructor which creates the object of the Admin Repository.
        *
        * @param    _context            A Database Context
        * @param    _userManager        An UserManager
        */
        public AdminRepository(DatabaseContext _context, UserManager<UserModel> _userManager)
        {
            this._context = _context;
            this._userManager = _userManager;
            //this._roleManager = _roleManager;
        }

        /**
        * CreateStaff service is to create staff.
        *
        * @param    model       A SignupModel
        * @return   status      A Status
        */
        public async Task<Status> CreateStaff(SignupModel model)
        {
            var status = new Status();
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var emailExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null || emailExists != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User already exists";
            }
            else
            {
                UserModel user = new()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Username,
                    EmailConfirmed = true

                };
                await _userManager.CreateAsync(user, model.Password);
                status.StatusCode = 1;
            }


            return status;
        }

        /**
        * DeleteBooking service is to delete a booking.
        *
        * @param    id          A String
        * @return   status      A Status
        */
        public async Task<Status> DeleteBooking(string id)
        {
            var status = new Status();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                status.StatusCode = 1;
            }
            else
            {
                status.StatusCode = 0;
            }
            return status;
        }

        /**
        * DeleteUser service is to delete a user.
        *
        * @param    id          A String
        * @return   status      A Status
        */
        public async Task<Status> DeleteUser(string id)
        {
            var status = new Status();

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                status.StatusCode = 1;
            }
            else
            {
                status.StatusCode = 0;
            }
            return status;
        }

        /**
        * GetAllBooking service is to get all booking.
        *
        * @return   bookingList      A list of BookingViewModel
        */
        public async Task<IEnumerable<BookingViewModel>> GetAllBooking()
        {
            var bookings = await _context.Bookings.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var bookingList = new List<BookingViewModel>();

            var userBooking =
                            from booking in bookings
                            join user in users on booking.UserId equals user.Id
                            join staff in users on booking.StaffId equals staff.Id

                            select new BookingViewModel
                            {
                                Id = booking.Id,
                                UserId = user.Id,
                                StaffId = booking.StaffId,
                                UserName = user.UserName,
                                StaffName = staff.UserName,
                                BookingTime = booking.BookingTime,
                                Status = booking.Status
                            };

            bookingList = userBooking
                .Where(b => b.Status == 0)
                .OrderBy(b => b.BookingTime)
                .Cast<BookingViewModel>()
                .ToList();

            return bookingList;
        }

        /**
        * GetAllUser service is to get all users.
        *
        * @return   nonAdminUserList      A List of UserModel
        */
        public async Task<IEnumerable<UserModel>> GetAllUser()
        {

            var nonAdminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            var allUsers = await _userManager.Users.ToListAsync();

            //var nonAdminUsersList = allUsers.Except(nonAdminUsers)
            //    .Where(user =>user.Name.ToLower().Contains(search) || // Search in the "Name" property
            //    user.UserName.ToLower().Contains(search) || // Search in the "UserName" property
            //    user.Email.ToLower().Contains(search)) // Search in the "email" property
            //    .OrderBy(user => user.UserName)
            //    .ToList();

            var nonAdminUsersList = allUsers.Except(nonAdminUsers)
                .OrderBy(user => user.UserName)
                .ToList();


            return nonAdminUsersList;

        }

        /**
        * GetAverageRating service is to get average rating of each staff.
        *
        * @return   userRatingList      A list of RatingViewModel
        */
        public async Task<IEnumerable<RatingViewModel>> GetAverageRating()
        {
            var ratings = await _context.Ratings.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var userRatingList = new List<RatingViewModel>();
            var userRating =
                from rating in ratings
                join user in users on rating.UserId equals user.Id
                join staff in users on rating.StaffId equals staff.Id
                group new { staff.Name, rating.Rating } by staff.Name into staffRatings
                select new RatingViewModel
                {
                    StaffName = staffRatings.Key,
                    AverageRating = Math.Round(staffRatings.Average(r => r.Rating), 2)
                };

            userRatingList = userRating
                .OrderBy(r => r.StaffName)
                .Cast<RatingViewModel>()
                .ToList();

            return userRatingList;
        }

        /**
        * GetBookingById service is to get a booking based on the id.
        *
        * @param    id      A String
        * @return   Booking A BookingModel
        */
        public async Task<BookingModel> GetBookingById(string id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        /**
        * GetUserById service is to get a user based on the id.
        *
        * @param    id      A String
        * @return   User    A UserModel
        */
        public async Task<UserModel> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);


        }

        /**
        * SelectUser service is to get the selected user information.
        *
        * @param    model           A BulkEmailViewModel
        * @return   selectedUsers   A List of selected UserModel
        */
        public async Task<IEnumerable<UserModel>> SelectUser(BulkEmailViewModel model)
        {
            var nonAdminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            var allUsers = await _userManager.Users.ToListAsync();

            var userList = allUsers.Except(nonAdminUsers)
                .ToList();

            List<string> selectedUserIds = model.SelectedUserIds;

            var selectedUsers = userList.Where(user => selectedUserIds.Contains(user.Id)).ToList();

            return selectedUsers;
        }

        //public Task SelectUser(BulkEmailViewModel model)
        //{
        //    var nonAdminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        //    var allUsers = await _userManager.Users.ToListAsync();


        //}

        //public async Task<IEnumerable<User>> SearchUser(string search, string user)
        //{
        //    if (user == "staff")
        //    {
        //        //staff
        //        var nonAdminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        //        // Invert the result to get non-admin users
        //        var allUsers = await _userManager.Users.ToListAsync();
        //        var nonAdminUsersList = allUsers.Except(nonAdminUsers).ToList();

        //        var nonUserUsers = await _userManager.GetUsersInRoleAsync("User");
        //        var staffList = nonAdminUsersList.Except(nonUserUsers)
        //            .Where(user => user.Name.ToLower().Contains(search) || // Search in the "Name" property
        //            user.UserName.ToLower().Contains(search) || // Search in the "UserName" property
        //            user.Email.ToLower().Contains(search)) // Search in the "email" property
        //             .OrderBy(user => user.UserName)
        //            .ToList();

        //        return staffList;
        //    }
        //    else
        //    {
        //        //user
        //        var nonAdminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        //        var allUsers = await _userManager.Users.ToListAsync();
        //        var nonAdminUsersList = allUsers.Except(nonAdminUsers)
        //            .Where(user => user.Name.ToLower().Contains(search) || // Search in the "Name" property
        //            user.UserName.ToLower().Contains(search) || // Search in the "UserName" property
        //            user.Email.ToLower().Contains(search)) // Search in the "email" property
        //            .OrderBy(user => user.UserName)
        //            .ToList();


        //        return nonAdminUsersList;
        //    }



        //}

        /**
        * UpdateUser service is to update the selected user.
        *
        * @param    model   A UserViewModel
        * @return   status  A Status Object
        */
        public async Task<Status> UpdateUser(UserViewModel model)
        {
            Status status = new Status();
            var users = await _context.Users.FindAsync(model.Id);
            Console.WriteLine(model.Id);

            if (users != null)
            {
                var userExists = await _userManager.FindByNameAsync(model.UserName);
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists == null|| emailExists == null)
                {
                    users.Name = model.Name;
                    users.UserName = model.UserName;
                    users.Email = model.Email;
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                    status.StatusCode = 1;
                }
                //if (userExists != null || emailExists != null)
                //{

                //}
                else
                {
                    status.StatusCode = 0;
                }

            }
            else
            {
                status.StatusCode = 0;
            }

            return status;
        }

        /**
        * UserModelToUserViewModel service is to convert UserModel to UserViewModel.
        *
        * @param    id  A String
        * @return       A UserViewModel
        */
        public async Task<UserViewModel> UserModelToUserViewModel(string id)
        {
            UserModel user = await _context.Users.FindAsync(id);
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
