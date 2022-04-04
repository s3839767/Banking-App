using Microsoft.AspNetCore.Mvc;
using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using McbaExampleWithLogin.Utilities;
using McbaExampleWithLogin.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using SimpleHashing;

namespace McbaExample.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class BillPayController : Controller
{
    private readonly McbaContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(McbaContext context) => _context = context;

    // Can add authorize attribute to actions.
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);

        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);

        return View(customer);
    }
    [HttpPost]
    public async Task<IActionResult> Index(int id, int accountNumber)
    {
        // show error for business rule
        if (accountNumber == 0)
        {
            return RedirectToAction(nameof(Index));
        }
        HttpContext.Session.SetInt32("id", accountNumber);



        return RedirectToAction(nameof(ViewBills), new {id = HttpContext.Session.GetInt32("id")});
    }

    public async Task<IActionResult> ViewBills(int id) => View(await _context.Accounts.FindAsync(id));
    //Method used to cancel a BillPay
    public async Task<IActionResult> BillPayCancel(int id)
    {
        var accountNumber = (int)HttpContext.Session.GetInt32("id");
        var account = await _context.Accounts.FindAsync(accountNumber);
        var billPay = await _context.BillPay.FindAsync(id);
        account.BillPays.Remove(billPay);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ViewBills), new { id = accountNumber });
    }

    
    public async Task<IActionResult> BillPayModify(int id)
    {
        var payees = await _context.Payee.Select(x => x.PayeeID).ToListAsync();
        ViewBag.Payees = payees;
        return View(await _context.BillPay.FindAsync(id));
    }

    //Method to modify bill pays with utc time conversion
    [HttpPost]
    public async Task<IActionResult> BillPayModify(int id, int payeeID, decimal amount, DateTime date, string period)
    {
        var payees = await _context.Payee.Select(x => x.PayeeID).ToListAsync();
        ViewBag.Payees = payees;
        var accountNumber = (int)HttpContext.Session.GetInt32("id");
        var account = await _context.Accounts.FindAsync(accountNumber);
        var billPay = await _context.BillPay.FindAsync(id);
        //checks that payee is selected
        if (payeeID == -1)
        {
            ModelState.AddModelError(nameof(payeeID), "Need to select PayeeID.");
        }
        //checks that amount is positive
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        if (date.CompareTo(DateTime.Now) < 0)
        {
            ModelState.AddModelError(nameof(date), "Scheduled date and time cannot be earlier than the current date and time.");
        }

        if (!ModelState.IsValid)
        {
            return View(billPay);
        }
        var dateUTC = date.ToUniversalTime();

        billPay.PayeeID = payeeID;
        billPay.Amount = amount;
        billPay.ScehduleTimeUtc = dateUTC;
        billPay.Period = char.Parse(period);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ViewBills), new { id = accountNumber });
    }

    public async Task<IActionResult> BillPayCreate(int id)
    {
        var payees = await _context.Payee.Select(x => x.PayeeID).ToListAsync();
        ViewBag.Payees = payees;

        return View(await _context.Accounts.FindAsync(id));
    }
    //Method for creating BillPay
    [HttpPost]
    public async Task<IActionResult> BillPayCreate(int id, int payeeID, decimal amount, DateTime date, string period)
    {
        var payees = await _context.Payee.Select(x => x.PayeeID).ToListAsync();
        ViewBag.Payees = payees;
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        if (date.CompareTo(DateTime.Now) < 0)
        {
            ModelState.AddModelError(nameof(date), "Scheduled date and time cannot be earlier than the current date and time.");
        }

        if (!ModelState.IsValid)
        {
            return View(account);
        }

        var dateUtc = date.ToUniversalTime();
        account.BillPays.Add(
            new BillPay
            {
                AccountNumber = id,
                PayeeID = payeeID,
                Amount = amount,
                ScehduleTimeUtc = dateUtc,
                Period = char.Parse(period)
            }); 

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ViewBills), new { id = id });
    }
}