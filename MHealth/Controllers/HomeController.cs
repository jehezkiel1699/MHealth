using MHealth.Models;
using MHealth.Models.Domain;
using MHealth.Models.Domain.View;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

/**
 * This Controller contains all home functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Controllers
{

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;

        /**
        * Default constructor which creates the object of the Home.
        *
        * @param    _userRepository  An Interface of User 
        */
        public HomeController(IUserRepository _userRepository)
        {
            //this._logger = _logger;
            this._userRepository = _userRepository;
        }

        /**
        * Index method to display the home.
        *
        * @return   Index View
        */
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}


        /**
        * Error method to display the error.
        *
        * @return   Error View
        */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /**
        * Staff method to display the staff list.
        *
        * @param    currentPage     a integer
        * @return   Staff View
        */
        public async Task<IActionResult> Staff(int currentPage = 1)
        {
            //search = string.IsNullOrEmpty(search) ? "" : search.ToLower();
            try
            {
                var users = await _userRepository.GetAllStaff();

                var totalCount = users.Count();
                int pageSize = 10;
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
            catch (Exception ex)
            {
                TempData["error"] = "1";
                return View();
            }
            
        }
    }
}