using MHealth.Migrations;
using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using MHealth.Repositories.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

/**
 * This Controller contains all staff functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Controllers
{
    [Authorize(Roles = "staff")]
    public class StaffController : Controller
    {
        //private readonly ILogger<BookingController> _logger;
        private readonly IStaffRepository _staffRepository;
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailRepository _emailRepository;

        /**
        * Default constructor which creates the object of the Staff.
        *
        * @param    _staffRepository    An Interface of Staff 
        * @param    _userManager        An Interface of User
        * @param    _emailRepository    An Interface of Email
        */
        public StaffController(IStaffRepository _staffRepository, UserManager<UserModel> _userManager, IEmailRepository _emailRepository)
        {
            //this._logger = _logger;
            this._staffRepository = _staffRepository;
            this._userManager = _userManager;
            this._emailRepository = _emailRepository;
        }

        /**
        * Index method to display the staff index.
        *
        * @param    currentPage     an Integer
        * @return   Index View
        */
        public async Task<IActionResult> Index(int currentPage = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                var bookings = await _staffRepository.GetAllBooking(user.Id);

                var totalCount = bookings.Count();
                int pageSize = 5;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                List<BookingViewModel> bookingList = bookings.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                BookingPaginationViewModel bookingData = new BookingPaginationViewModel()
                {
                    Bookings = bookingList,
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    PageSize = pageSize
                };
                return View(bookingData);
            }
            catch (Exception ex)
            {
                TempData["error"] = "1";
            }
            return View();
        }

        /**
        * Detail method to display the detail view.
        *
        * @param    bookingId   a String
        * @param    userId      a String
        * @param    staffId     a String
        * @return   Detail View
        */
        [HttpPost]
        public async Task<IActionResult> Detail(string bookingId, string userId, string staffId)
        {
            UserModel user = await _staffRepository.GetUserInformation(userId);
            ViewBag.UserName = user.UserName;
            var booking = new MRIPostModel()
            {
                BookingId = bookingId,
                UserId = userId,
                StaffId = staffId
            };
            return View(booking);
        }

        /**
        * Post method to post the result of mri.
        *
        * @param    model       a MRIPostModel
        * @param    postImage   an IFormFile
        * @param    description a String
        * @return   True:Index View
        *           False:Detail View
        */
        [HttpPost]
        public async Task<IActionResult> Post(MRIPostModel model, IFormFile postImage, string description)
        {

            if (postImage == null)
            {
                return View(nameof(Detail), new MRIPostModel
                {
                    BookingId = model.BookingId,
                    UserId = model.UserId,
                    StaffId = model.StaffId
                });
            }
            try
            {
                MRIPostModel postModel = await _staffRepository.WriteFile(postImage);

                //Console.WriteLine(fileName);
                model.Description = description;
                model.Id = Guid.NewGuid().ToString();
                //Console.WriteLine(fileName.ToString());
                model.PostPath = postModel.PostPath.ToString();

                await _staffRepository.Post(model);
                Console.WriteLine(model.BookingId);
                var bookingDetail = await _staffRepository.GetBookingDetail(model.BookingId);
                await _emailRepository.SendEmail(bookingDetail.StaffEmail, bookingDetail.UserEmail, "MRI RESULT", description, model.PostPath);

                //return View(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(nameof(Detail), new MRIPostModel
                {
                    BookingId = model.BookingId,
                    UserId = model.UserId,
                    StaffId = model.StaffId
                });
            }
        }
    }
}
