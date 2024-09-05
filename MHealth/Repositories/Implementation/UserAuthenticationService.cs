using MHealth.Models.Domain;
using MHealth.Models.DTO;
using MHealth.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

/**
 * This Repository stores all User Authentication services
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<UserModel> signInManager;
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        /**
        * Default constructor which creates the object of the User Authentication Repository.
        *
        * @param    signInManager   A SignInManager
        * @param    userManager     A UserManager
        * @param    RoleManager     A RoleManager
        */
        public UserAuthenticationService(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /**
        * GoogleCallback service is to login by using google.
        *
        * @param    returnUrl       A String
        * @param    remoteError     A String
        * @return   status          A Status
        */
        public async Task<Status> GoogleCallback(string returnUrl, string remoteError)
        {
            var status = new Status();
            LoginModel loginViewModel = new LoginModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                                    (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                status.StatusCode = 0;
                return status;
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                status.StatusCode = 0;
                return status;

            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                status.StatusCode = 1;
                return status;
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new UserModel
                        {
                            Name = info.Principal.FindFirstValue(ClaimTypes.Email),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    try
                    {

                        await userManager.AddLoginAsync(user, info);
                        await userManager.AddToRoleAsync(user, "user");
                        await signInManager.SignInAsync(user, isPersistent: false);


                        var userRoles = await userManager.GetRolesAsync(user);
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName)
                        };
                        } catch (Exception ex)
                        {
                            status.StatusCode = 0;
                            return status;
                        }   

                }
                status.StatusCode = 1;
                return status;
            }
        }

        /**
        * LoginAsync service is to login with the stored user.
        *
        * @param    model   A LoginModel
        * @return   status  A Status
        */
        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Username or password is wrong";
                return status;
            }
            else
            {
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                {
                    status.StatusCode = 0;
                    status.StatusMessage = "Username or password is wrong";
                    return status;
                }
                else
                {
                    var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);

                    if (signInResult.Succeeded)
                    {
                        var userRoles = await userManager.GetRolesAsync(user);
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName)
                        };
                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }

                        status.StatusCode = 1;
                        status.StatusMessage = "Logged in succcessfully";
                        return status;

                    }

                    else if (signInResult.IsLockedOut)
                    {
                        status.StatusCode = 0;
                        status.StatusMessage = "User locked out";
                        return status;
                    }
                    else
                    {
                        status.StatusCode = 0;
                        status.StatusMessage = "Error on logging in";
                        return status;
                    }
                }
            }

            
        }

        /**
        * LogoutAsync service is to logout.
        *
        */
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        /**
        * RegistrationAsync service is register the new user.
        *
        * @param    model   A SignupModel
        * @return   status  A Status
        */
        public async Task<Status> RegistrationAsync(SignupModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            var emailExists = await userManager.FindByEmailAsync(model.Email);

            if (userExists != null || emailExists != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User already exists";
                return status;
            }

            UserModel user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User creation failed";
                return status;
            }
            else
            {
                if (!await roleManager.RoleExistsAsync(model.Role))
                    await roleManager.CreateAsync(new IdentityRole(model.Role));
                else
                {
                    await userManager.AddToRoleAsync(user, model.Role);
                }
                var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);

                if (signInResult.Succeeded)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName)
                        };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    status.StatusCode = 1;

                }
                return status;

            }
        }
    }
}
