using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using Microsoft.AspNetCore.Mvc;

namespace McbaExampleWithLogin.Controllers;



public class MyStatementsController : Controller
{
    private readonly McbaContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public MyStatementsController(McbaContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);

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
        return RedirectToAction(nameof(ViewStatements), new {id = accountNumber, start = 0, end = 3, currentPage = 1});
    }

    //Allows
    public async Task<IActionResult> ViewStatements(int id, int start, int end, int currentPage)
    {
        HttpContext.Session.SetInt32("start", start);
        HttpContext.Session.SetInt32("end", end);
        HttpContext.Session.SetInt32("accountnumber", id);
        HttpContext.Session.SetInt32("currentPage", currentPage);
        var account = await _context.Accounts.FindAsync(id);
        if(end > account.Transactions.Count - 1)
        {
            HttpContext.Session.SetInt32("end", account.Transactions.Count - 1);
        }

        account.Transactions = account.Transactions.OrderByDescending(e => e.TransactionTimeUtc).ToList();
        int transactionNumber = 0;

       
        foreach (var transaction in account.Transactions)
        {
            switch (transaction.TransactionType)
            {
                case 'W':
                    HttpContext.Session.SetString("type" + transactionNumber.ToString(), "Withdraw");
                    break;
                case 'D':
                    HttpContext.Session.SetString("type" + transactionNumber.ToString(), "Deposit");
                    break;
                case 'T':
                    HttpContext.Session.SetString("type" + transactionNumber.ToString(), "Transfer");
                    break;
                case 'S':
                    HttpContext.Session.SetString("type" + transactionNumber.ToString(), "Service Charge");
                    break;
                case 'B':
                    HttpContext.Session.SetString("type" + transactionNumber.ToString(), "BillPay");
                    break;
            }
            transactionNumber++;
        }



        return View(account);
    }
    //Previous page of transactions
    public async Task<IActionResult> Back(int id) 
    {
        id = (int)HttpContext.Session.GetInt32("accountnumber");
        var account = await _context.Accounts.FindAsync(id);

        if((int)HttpContext.Session.GetInt32("currentPage") == ((int)Math.Ceiling((decimal)account.Transactions.Count() / 4)))
        {
            HttpContext.Session.SetInt32("end", (int)HttpContext.Session.GetInt32("start") - 1);
        }
        else
        {
           HttpContext.Session.SetInt32("end", (int)HttpContext.Session.GetInt32("end") - 4);
        }

        return RedirectToAction(nameof(ViewStatements), new { id, start = (int)HttpContext.Session.GetInt32("start") - 4, end = (int)HttpContext.Session.GetInt32("end"), currentPage = (int)HttpContext.Session.GetInt32("currentPage") - 1});
    }
    //Next page of transactions
    public async Task<IActionResult> Next(int id)
    {
        id = (int)HttpContext.Session.GetInt32("accountnumber");
        return RedirectToAction(nameof(ViewStatements), new { id, start = (int)HttpContext.Session.GetInt32("start") + 4, end = (int)HttpContext.Session.GetInt32("end") + 4, currentPage = (int)HttpContext.Session.GetInt32("currentPage") + 1 });
    }

    // use viewbag for transactions 

}
    
