using Microsoft.AspNetCore.Mvc;
using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using SimpleHashing;

namespace McbaExampleWithLogin.Controllers;

//Taken from Lectorial 6
[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    private readonly McbaContext _context;

    public LoginController(McbaContext context) => _context = context;

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(int loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
            if(login == null || string.IsNullOrEmpty(password) || !PBKDF2.Verify(login.PasswordHash, password))
            { 
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            else if(login.IsLocked == true)
            {
                ModelState.AddModelError("LoginLocked", "Login locked, please try contact an admin to get your account unlocked.");
                return View(new Login { LoginID = loginID });
            }
        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
