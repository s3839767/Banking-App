using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
//using SimpleHashing;

namespace MvcMovie.Controllers;


[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        if(!loginID.Equals("admin"))
            ModelState.AddModelError(nameof(loginID), "Admin Username Incorrect.");
        if (!password.Equals("admin"))
            ModelState.AddModelError(nameof(password), "Admin Password Incorrect.");
        if (!ModelState.IsValid)
        {
            return View();
        }
        HttpContext.Session.SetInt32("LoggedIn", 1);
        return RedirectToAction("Index", "Home");
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
