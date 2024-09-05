using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MHealth.Models.Domain;
using Microsoft.AspNetCore.Identity;

/**
 * This Controller contains all user functions
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserAuthenticationService _service;
        //private readonly ILogger _logger;
        private readonly SignInManager<UserModel> _signInManager;

        /**
        * Default constructor which creates the object of the Use.
        *
        * @param    _service            an Interface of User
        * @param    _signInManager      a SignInManager
        */
        public UserController(IUserAuthenticationService _service, SignInManager<UserModel> _signInManager)
        {
            this._service = _service;
            //this._logger = _logger;
            this._signInManager = _signInManager;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}


        /**
        * Signup method to display the signup view.
        *
        * @return Signup View
        */
        public IActionResult Signup()
        {
            return View();
        }

        /**
        * Login method to display the login view.
        *
        * @return Login View
        */
        public IActionResult Login()
        {

            return View();
        }

        /**
        * Signup method to signup.
        *
        * @param    model   a SignupModel
        * @return   True:Home View
        *           False:Signup View
        */
        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Role = "user";
            var result = await _service.RegistrationAsync(model);
            //TempData["msg"] = result.StatusMessage;
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //TempData["msg"] = result.StatusMessage;
                //TempData["name"] = User.Identity.Name;
                return RedirectToAction(nameof(Signup));

            }
        }

        /**
        * Login method to login with the registered user.
        *
        * @param    model   a LoginModel
        * @return   True:Home View
        *           False:Login View
        */
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _service.LoginAsync(model);
            //TempData["msg"] = result.StatusMessage;
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //TempData["msg"] = result.StatusMessage;
                //TempData["name"] = User.Identity.Name;
                return RedirectToAction(nameof(Login));
            }

        }

        /**
        * Login method to login with external api.
        *
        * @param    returnUrl   a String
        * @return Login View
        */
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginModel model = new LoginModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        /**
        * GoogleLogin method to login with google.
        *
        * @param    provider    a String
        * @param    returnUrl   a String
        * @return   ChallengeResult
        */
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GoogleLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("GoogleCallback", "User",
                                    new { ReturnUrl = returnUrl });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        /**
        * GoogleCallback method to login with google.
        *
        * @param    returnUrl   a String
        * @param    remoteError a String
        * @return   True:Index View
        *           False:Login View
        */
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var result = await _service.GoogleCallback(returnUrl, remoteError);
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //TempData["msg"] = result.StatusMessage;
                //TempData["name"] = User.Identity.Name;
                return RedirectToAction(nameof(Login));
            }

        }

        /**
        * Logout method to logout.
        *
        * @return Index View
        */
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            TempData.Remove("msg");
            return RedirectToAction("Index", "Home");
        }


        
    }
}
