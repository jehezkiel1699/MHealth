using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

/**
 * This Controller contains all booking functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Controllers
{
    //[Authorize]
    [Authorize(Roles = "user")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<UserModel> _userManager;

        /**
        * Default constructor which creates the object of the Booking.
        *
        * @param    _bookingRepository  An Interface of Booking 
        * @param    _userManager        A User Manager
        */
        public BookingController(IBookingRepository _bookingRepository, UserManager<UserModel> _userManager)
        {
            this._bookingRepository = _bookingRepository;
            this._userManager = _userManager;
        }

        /**
        * Index method to display the booking list.
        *
        * @param    currentPage     an Integer
        * @return   Index View
        */
        public async Task<IActionResult> Index(int currentPage = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                var bookings = await _bookingRepository.GetAllBooking(user.Id);

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
        * Booking method to display booking view with the booking's date.
        *
        * @param    id     a String
        * @return   Booking View
        */
        public async Task<IActionResult> Booking(string id)
        {

            BookingModel bookingModel = new BookingModel
            {
                StaffId = id // Set the id in the BookingModel
            };

            return View(bookingModel);
        }

        /**
        * CreateBooking method to book a booking.
        *
        * @param    id              a String
        * @param    selectedDate    a String
        * @return   True:Staff View
        *           False:Booking View
        */
        [HttpPost]
        public async Task<IActionResult> CreateBooking(string id, string selectedDate)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                DateTime bookingTime = DateTime.Parse(selectedDate);
                //DateTime dateOnly = new DateTime(bookingTime.Year, bookingTime.Month, bookingTime.Day, 0, 0, 0);
                //TempData["selectedDate"] = bookingTime;
                try
                {

                    await _bookingRepository.Booking(user.Id, id, bookingTime);
                    return RedirectToAction("Staff", "Home");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Booking", "Booking");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Booking", "Booking");

            }

        }

        /**
        * Detail method to display the detail view.
        *
        * @param    bookingId       a String
        * @param    userId          a String
        * @param    staffId         a String
        * @return   Detail View
        */
        [HttpPost]
        public async Task<IActionResult> Detail(string bookingId, string userId, string staffId)
        {
            MRIViewModel mri = await _bookingRepository.GetBookingInformation(bookingId, userId, staffId);
            if(mri == null)
            {
                TempData["error"] = "1";
            }
            return View(mri);
        }

        /**
        * Rating method to rating the finished booking.
        *
        * @param    bookingId       a String
        * @param    userId          a String
        * @param    staffId         a String
        * @param    rating          an integer
        * @return   True:Index View
        * @return   False:Detail View
        */
        [HttpPost]
        public async Task<IActionResult> Rating(string bookingId, string userId, string staffId, int rating)
        {

            if (rating == 0)
            {
                MRIViewModel mri = await _bookingRepository.GetBookingInformation(bookingId, userId, staffId);
                return View("Detail", mri);

            }
            await _bookingRepository.RateStaff(bookingId, userId, staffId, rating);
            return RedirectToAction(nameof(Index));
        }

    }
}
