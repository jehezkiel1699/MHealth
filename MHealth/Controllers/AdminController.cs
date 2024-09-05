using CoreHtmlToImage;
using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;

/**
 * This Controller contains all admin functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */

namespace MHealth.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        //private readonly ILogger<AdminController> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _configuration;


        /**
        * Default constructor which creates the object of the Admin.
        *
        * @param    _adminRepository        An Interface of Admin 
        * @param    _emailRepository        An Interface of Email
        * @param    _configuration          A configuration
        */
        public AdminController(IAdminRepository _adminRepository, IEmailRepository _emailRepository, IConfiguration _configuration)
        {
            //this._logger = _logger;
            this._adminRepository = _adminRepository;
            this._emailRepository = _emailRepository;
            this._configuration = _configuration;
        }

        /**
        * Announcement method to get the view of announcement.
        *
        * @return   Announcement View
        */
        public async Task<IActionResult> Announcement()
        {
            try
            {
                var users = await _adminRepository.GetAllUser();
                ViewBag.Users = users.ToList();
                //var viewModel = new BulkEmailViewModel
                //{
                //    Users = users// Initialize as empty
                //};
                return View();
                //return View(new BulkEmailViewModel()
                //{
                //    Users = users.ToList()
                //});
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["error"] = 1;
                return View();
            }
        }

        /**
        * Announce method to send an announcement to the selected user.
        *
        * @param    model       BulkEmailViewModel
        * @return   True:Index View
        *           False:Announcement View
        */
        [HttpPost]
        public async Task<IActionResult> Announce(BulkEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selectedUsers = await _adminRepository.SelectUser(model);
                Console.WriteLine(selectedUsers);
                foreach (var user in selectedUsers)
                {
                    await _emailRepository.SendEmail(_configuration["AdminEmail"], user.Email, model.Subject, model.Announcement);
                }

                return RedirectToAction(nameof(Index));

            }
            var users = await _adminRepository.GetAllUser();
            ViewBag.Users = users.ToList();
            return View(nameof(Announcement));
        }

        /**
        * Index method to display the admin index.
        *
        * @param    currentPage     an Integer
        * @return   Index View
        */
        public async Task<IActionResult> Index(int currentPage = 1)
        {
            //search = string.IsNullOrEmpty(search) ? "" : search.ToLower();
            var users = await _adminRepository.GetAllUser();
            var totalCount = users.Count();
            int pageSize = 5;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            List<UserModel> userList = users.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            UserPaginationViewModel userData = new UserPaginationViewModel()
            {
                Users = userList,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return View(userData);
        }

        /**
        * Booking method to display the booking list.
        *
        * @param    currentPage     an Integer
        * @return   True:Booking View with data
        *           False:Booking View without data
        */
        public async Task<IActionResult> Booking(int currentPage = 1)
        {
            try
            {
                var bookings = await _adminRepository.GetAllBooking();

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
        * Chart method to display the rating chart.
        *
        * @return   Chart View with the average rating each staff
        */
        public async Task<IActionResult> Chart()
        {
            try
            {
                IEnumerable<RatingViewModel> userRating = await _adminRepository.GetAverageRating();

                return View(userRating);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        /**
        * EditUser method to get the information editted user.
        *
        * @param    id      a String
        * @return   True:EditUser View
        *           False:Index View
        */
        public async Task<IActionResult> EditUser(string id)
        {
            UserViewModel userViewModel = await _adminRepository.UserModelToUserViewModel(id);
            if (userViewModel != null)
            {
                return View(userViewModel); // Pass the user data to the view
            }
            //TempData["msg"] = $"User is not found with Id : {id}";
            return RedirectToAction(nameof(Index));
        }

        /**
        * EditUser method to make the changes of user.
        *
        * @param    model      a UserViewModel
        * @return   True:Index View
        *           False:EditUser View
        */
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _adminRepository.UpdateUser(model);
                    //TempData["code"] = "1";
                    //TempData["msg"] = "The user updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    //TempData["code"] = "0";
                    //TempData["msg"] = "The user is failed to be updated";
                    return View();
                }
            }
            return View();
        }

        /**
        * DeleteUser method to delete the selected user.
        *
        * @param    id      a String
        * @return   Index View
        */
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                UserModel user = await _adminRepository.GetUserById(id);

                if (user != null)
                {
                    try
                    {
                        await _adminRepository.DeleteUser(id);
                        //return RedirectToAction(nameof(Index));

                        //TempData["msg"] = "The user deleted successfully";
                    }
                    catch (Exception ex)
                    {
                        //TempData["msg"] = ex.Message;

                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["msg"] = ex.Message;

            }
            TempData["msg"] = $"User is not found with Id : {id}";
            return RedirectToAction(nameof(Index));

        }

        /**
        * DeleteBooking method to delete the selected booking.
        *
        * @param    id      a String
        * @return   True:Booking View
        */
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            try
            {
                BookingModel user = await _adminRepository.GetBookingById(id);

                if (user != null)
                {
                    try
                    {
                        await _adminRepository.DeleteBooking(id);
                        //return RedirectToAction(nameof(Index));

                        //TempData["msg"] = "The user deleted successfully";
                    }
                    catch (Exception ex)
                    {
                        //TempData["msg"] = ex.Message;

                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["msg"] = ex.Message;

            }
            TempData["msg"] = $"Booking is not found with Id : {id}";
            return RedirectToAction(nameof(Booking));

        }

        /**
        * CreateStaff method display the create view.
        *
        * @return  CreateStaff View
        */
        public IActionResult CreateStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Role = "staff";
            try
            {
                await _adminRepository.CreateStaff(model);
                //TempData["msg"] = "The user created successfully";
                //TempData["name"] = User.Identity.Name;
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                //TempData["msg"] = "The user is failed to be updated";

                return RedirectToAction(nameof(CreateStaff));
            }

        }

        /**
        * GeneratePDF method is to generate chart to pdf.
        *
        * @return  Chart View
        */
        [HttpPost]
        public async Task<IActionResult> GeneratePDF()
        {
            try
            {
                var Renderer = new IronPdf.ChromePdfRenderer();
                Renderer.RenderingOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Landscape;
                //Renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A2;
                //Renderer.RenderingOptions.ViewPortWidth = 1200;
                var doc = Renderer.RenderUrlAsPdf("https://localhost:7201/Admin/GeneratedChart")
                    .SaveAs("generatedChart.pdf");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            IEnumerable<RatingViewModel> userRating = await _adminRepository.GetAverageRating();
            return View("Chart", userRating);
        }

        /**
        * GenerateJPG method is to generate chart to jpg.
        *
        * @return  Chart View
        */
        [HttpPost]
        public async Task<IActionResult> GenerateJPG()
        {
            try
            {
                var converter = new HtmlConverter();
                var bytes = converter.FromUrl("https://localhost:7201/Admin/GeneratedChart");
                System.IO.File.WriteAllBytes("generatedChart.jpg", bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            IEnumerable<RatingViewModel> userRating = await _adminRepository.GetAverageRating();
            return View("Chart", userRating);
        }

        /**
        * GeneratedChart method is used by generated.
        *
        * @return   Chart View with the average rating each staff
        */
        [AllowAnonymous]
        public async Task<IActionResult> GeneratedChart()
        {
            try
            {
                IEnumerable<RatingViewModel> userRating = await _adminRepository.GetAverageRating();
                return View(userRating);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

    }
}



