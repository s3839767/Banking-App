using Microsoft.AspNetCore.Mvc;
using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using McbaExampleWithLogin.Utilities;
using McbaExampleWithLogin.Filters;
using System.Text;
using SimpleHashing;
using Microsoft.EntityFrameworkCore;

namespace McbaExampleWithLogin.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly McbaContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaContext context) => _context = context;

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

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        //Format constraints for  Deposits
        if(amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if(amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment too long please ensure it is less than 30 characters long!");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }
        // change the way comment is being passed through
        return RedirectToAction(nameof(Confirmation), new { id = id, amount = amount, transactionType = 'D', comment = comment});
    }

    public async Task<IActionResult> Withdraw(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, decimal amount, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        //Format constraints for  Withdrawing
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (amount > account.Balance || (account.AccountType.Equals('C') && account.Balance - amount < 300))
        {
            ModelState.AddModelError(nameof(amount), "You cannot withdraw more than what your balance currently contains or is required to contain for its minimum.");
        }
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment too long please ensure it is less than 30 characters long!");
        if (!ModelState.IsValid)
        {

            ViewBag.Amount = amount;
            return View(account);
        }
        return RedirectToAction(nameof(Confirmation), new { id = id, amount = amount, transactionType = 'W' , comment = comment});
    }

    public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Transfer(int id, int idTo, decimal amount, string comment)
    {
        var from = await _context.Accounts.FindAsync(id);
        var to = await _context.Accounts.FindAsync(idTo);
        //Format constraints for  Transfers
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
         
        if (amount > from.Balance || (from.AccountType.Equals('C') && from.Balance - amount < 300))
        {
            ModelState.AddModelError(nameof(amount), "Cannot transfer more than what your balance currently contains or is required to contain for its minimum.");
        }
        if (to == null)
            ModelState.AddModelError(nameof(idTo), "Account to transfer to doesn't exist please try again!");
        if (to == from)
        {
            ModelState.AddModelError(nameof(idTo), "Destination account cannot be the same as the Source account");
        }
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Comment too long please ensure it is less than 30 characters long!");

        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.idTo = idTo;
            return View(from);
        }
        HttpContext.Session.SetInt32("idTo", idTo);
        return RedirectToAction(nameof(Confirmation), new { id = id, amount = amount, transactionType = 'T', comment = comment });
    }
    //Method to create transactions in the database.
    public async Task<IActionResult> Create(int id, decimal amount, char transactionType, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (transactionType == 'D')
        {
            account.Balance += amount;
        }
        else if (transactionType == 'W')
        {
            account.Balance -= amount;
        }
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = transactionType,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow,
                Comment = comment
            });

        await _context.SaveChangesAsync();

        if (transactionType == 'W')
        {
            return RedirectToAction(nameof(CheckServiceFee), new { id = id, chosenFeature = "Withdraw" });
        }

        return RedirectToAction(nameof(Index));
    }

    //Creates a transfer transaction
    public async Task<IActionResult> CreateTransfer(int id, int idTo, decimal amount, char transactionType, string comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        var accountTo = await _context.Accounts.FindAsync(idTo);
        account.Balance -= amount;
        accountTo.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = transactionType,
                DestinationAccountNumber = idTo,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow,
                Comment = comment
            });
        accountTo.Transactions.Add(
            new Transaction
            {
                TransactionType = transactionType,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow,
                Comment = comment
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(CheckServiceFee), new { id = id, chosenFeature = "Transfer"});
    }
    //Confirmation page setup
    public async Task<IActionResult> Confirmation(int id, decimal amount, char transactionType, string comment)
    {
        HttpContext.Session.SetString("amount", amount.ToString());
        HttpContext.Session.SetString("transactionType", transactionType.ToString());
        if (comment != null)
            HttpContext.Session.SetString("comment", comment);
        else
            HttpContext.Session.SetString("comment", "");

        if (transactionType.Equals('W'))
        {
            HttpContext.Session.SetString("transactionName", "Withdraw");
        }
        else if (transactionType.Equals('D'))
        {
            HttpContext.Session.SetString("transactionName", "Deposit");
        }

        else if (transactionType.Equals('T'))
        {
            HttpContext.Session.SetString("transactionName", "Transfer");
        }

        return View(await _context.Accounts.FindAsync(id));
    }
    //Takes Transaction information and passes to respective create methods
    [HttpPost]
    public async Task<IActionResult> Confirmation(int id, Account account)
    {

        account = await _context.Accounts.FindAsync(id);
        if(HttpContext.Session.GetString("transactionType").Equals("T"))
        {
            return RedirectToAction(nameof(CreateTransfer), new { id = id, idTo = HttpContext.Session.GetInt32("idTo"), amount = HttpContext.Session.GetString("amount"), transactionType = HttpContext.Session.GetString("transactionType"), comment = HttpContext.Session.GetString("comment")});
        }
        return RedirectToAction(nameof(Create), new { id = id, amount = HttpContext.Session.GetString("amount"), transactionType = HttpContext.Session.GetString("transactionType"), comment = HttpContext.Session.GetString("comment") });
    }
    public async Task<IActionResult> MyProfileEdit(int id) => View(await _context.Customers.FindAsync(id));
    //Method for editing profile
    [HttpPost]
    public async Task<IActionResult> MyProfileEdit(int id, string name, string tfn, string address, string city, string state, string postcode, string mobile)
    {
        //All check constraints for Customer information
        var customer = await _context.Customers.FindAsync(id);
        if (name.Length > 50)
            ModelState.AddModelError(nameof(name), "Name must be less than 50 characters long.");
        if (tfn != null && tfn.Length > 11)
            ModelState.AddModelError(nameof(tfn), "TFN number should be 11 characters or less.");
        if (address != null && address.Length > 50)
            ModelState.AddModelError(nameof(address), "Address can't be more than 50 characters long");
        if (city != null && city.Length > 40 )
            ModelState.AddModelError(nameof(city), "City cant be more than 40 characters long");
        if (state != null && !(state.Length == 2 || state.Length == 3))
            ModelState.AddModelError(nameof(state), "State should only be 2 or 3 characters long, please use abreviation for the state!");
        
        if (postcode != null && postcode.Length != 4)
            ModelState.AddModelError(nameof(postcode), "Postcode can't be more than 4 characters long.");
        if (mobile != null && !(mobile.Length == 10))
            ModelState.AddModelError(nameof(mobile), "Mobile number should only have 10 numbers");
        if (mobile != null && !(mobile[0] == 0 && mobile[1] == 4))
        {
            ModelState.AddModelError(nameof(mobile), "Mobile number needs to start with '04'");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Name = name;
            ViewBag.Tfn = tfn;
            ViewBag.Address = address;
            ViewBag.City = city;
            ViewBag.State = state;
            ViewBag.PostCode = postcode;
            ViewBag.Mobile = mobile;
            return View(customer);
        }
        
        customer.Name = name;
        customer.TFN = tfn;
        customer.Address = address;
        customer.City = city;
        customer.State = state;
        customer.PostCode = postcode;
        customer.Mobile = mobile;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyProfile));
    }
    public async Task<IActionResult> MyProfile()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        return View(customer);
    }
    public async Task<IActionResult> PasswordEdit(int id) => View(await _context.Customers.FindAsync(id));
    [HttpPost]
    //Method to edit password
    public async Task<IActionResult> PasswordEdit(int id, string password, string passwordConfirm)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (password.Length > 8)
            ModelState.AddModelError(nameof(password), "password must be less than 8 characters long.");
        if (!password.Equals(passwordConfirm))
            ModelState.AddModelError(nameof(passwordConfirm), "The Passwords do not match.");

        if (!ModelState.IsValid)
        {
            return View(customer);
        }
        //Password hashing
        var hash = PBKDF2.Hash(password, 50000, 32);
        customer.Login.PasswordHash = hash.ToString();
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyProfile));
    }

    //method to check if Service Fee is required and what type it is.
    public async Task<IActionResult> CheckServiceFee(int id, string chosenFeature)
    {
        var account = await _context.Accounts.FindAsync(id);
        var transactions = await _context.Transactions.Where(x => x.AccountNumber == id).ToListAsync();
        int transactionAmount = 0;

        foreach(var transaction in transactions)
        {
            if (transaction.TransactionType.Equals('W'))
            {
                transactionAmount++;
            }
            else if (transaction.TransactionType.Equals('T') && transaction.DestinationAccountNumber != null)
            {
                transactionAmount++;
            }
        }

        if (transactionAmount > 2)
        {
            decimal fee = 0;
            if (chosenFeature.Equals("Withdraw"))
            {
                fee = (decimal)0.05;        
            }
            else if (chosenFeature.Equals("Transfer"))
            {
                fee = (decimal)0.10;

            }
            if ((account.Balance - fee) > 0)
            {
                account.Balance -= fee;
                account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = 'S',
                            AccountNumber = id,
                            Amount = fee,
                            TransactionTimeUtc = DateTime.UtcNow
                        });
            }


        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}