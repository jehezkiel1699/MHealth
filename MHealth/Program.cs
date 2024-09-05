using MHealth.Models.Domain;
using MHealth.Repositories.Abstract;
using MHealth.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(op => op.LoginPath = "/User/Login");

builder.Services
    .AddScoped<IUserAuthenticationService, UserAuthenticationService>()
    .AddScoped<IAdminRepository, AdminRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IBookingRepository, BookingRepository>()
    .AddScoped<IStaffRepository, StaffRepository>()
    .AddScoped<IEmailRepository, EmailRepository>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10); // Adjust the timeout as needed
    options.Cookie.IsEssential = true;
});

//options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        Console.WriteLine(configuration["Authentication:Google:ClientId"]);
        Console.WriteLine(configuration["Authentication:Google:ClientSecret"]);
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        //googleOptions.CallbackPath = "/signin-google";

    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();





//add 3 roles to database
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "admin", "staff", "user" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

//assign admin role to the user
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

    string username = configuration["AdminUsername"];
    string email = configuration["AdminEmail"];
    string password = configuration["AdminPassword"];



    try
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            //var passwordHasher = new PasswordHasher<IdentityUser>();

            var user = new UserModel
            {
                UserName = username,
                Name = username,
                Email = email
            };

            //user.PasswordHash = passwordHasher.HashPassword(user, password); ;

            await userManager.CreateAsync(user, password);

            await userManager.AddToRoleAsync(user, "Admin");

            //set admin role
        }
    }
    catch (Exception e)
    {
        throw new Exception("An error occurred while creating an admin: " + e.Message);
    }


}

//user
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

    try
    {


        var userData = new[]
        {
            new { UserName = "c14170054", Email = "c14170054@alumni.petra.ac.id"},
            new { UserName = "samsungs6alo", Email = "samsungs6alo@gmail.com"},
            new { UserName = "jehezkieltandijaya16", Email = "jehezkieltandijaya16@gmail.com" }
            // Add more user data as needed
        };

        foreach (var userEntry in userData)
        {
            string username = userEntry.UserName;
            string email = userEntry.Email;
            string password = configuration["UserPassword"];
            //string password = userEntry.Password;

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new UserModel
                {
                    UserName = username,
                    Name = username,
                    Email = email
                };

                await userManager.CreateAsync(user, password);

                // You can assign roles here if needed
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
    catch (Exception e)
    {
        throw new Exception("An error occurred while creating users: " + e.Message);
    }
}


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

    try
    {


        var userData = new[]
        {
            new { UserName = "jtandijaya16", Email = "jtandijaya16@gmail.com" },
            //new { UserName = "jehezkiel.ht16", Email = "jehezkiel.ht16@gmail.com"},
            new { UserName = "nxocrewgm1", Email = "nxocrewgm1@gmail.com" }
            // Add more user data as needed
        };

        foreach (var userEntry in userData)
        {
            string username = userEntry.UserName;
            string email = userEntry.Email;
            string password = configuration["UserPassword"];
            //string password = userEntry.Password;

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new UserModel
                {
                    UserName = username,
                    Name = username,
                    Email = email
                };

                await userManager.CreateAsync(user, password);

                // You can assign roles here if needed
                await userManager.AddToRoleAsync(user, "Staff");
            }
        }
    }
    catch (Exception e)
    {
        throw new Exception("An error occurred while creating users: " + e.Message);
    }
}

app.Run();
