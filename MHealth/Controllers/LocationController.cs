
using Microsoft.AspNetCore.Mvc;

/**
 * This Controller contains all location functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Controllers
{
    public class LocationController : Controller
    {
        private readonly IConfiguration _configuration;

        /**
        * Default constructor which creates the object of the Location.
        *
        * @param    _configuration          A configuration
        */
        public LocationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /**
        * Index method to display the location.
        *
        * @return   Index View
        */
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
