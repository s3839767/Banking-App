using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Newtonsoft.Json;
using System.Text;
//using SimpleHashing;

namespace MvcMovie.Controllers;

// Bonus Material: Implement global authorisation check.
//[AllowAnonymous]
public class AdminController : Controller
{

    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");
    public AdminController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public IActionResult Index() => View();

    //Webpage for selecting which customer Admin wishes to select
    public async Task<IActionResult> UserSelect()
    {
        var response = await Client.GetAsync($"api/Customers");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
        return View(customers);
    }

    //Modify profile page.
    public async Task<IActionResult> ModifyProfile(int id)
    {
        var response = await Client.GetAsync($"api/Customers/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<Customer>(result);
        return View(customers);
    }
    //Method for accessing API endpoint to update customer information
    [HttpPost]
    public async Task<IActionResult> ModifyProfile(int id, string name, string tfn, string address, string city, string state, string postcode, string mobile)
    {
        if (name.Length > 50)
            ModelState.AddModelError(nameof(name), "Name must be less than 50 characters long.");
        if (tfn != null && tfn.Length > 11)
            ModelState.AddModelError(nameof(tfn), "TFN number should be 11 characters or less.");
        if (address != null && address.Length > 50)
            ModelState.AddModelError(nameof(address), "Address can't be more than 50 characters long");
        if (city != null && city.Length > 40)
            ModelState.AddModelError(nameof(city), "City cant be more than 40 characters long");
        if (state != null && state.Length > 3)
            ModelState.AddModelError(nameof(state), "State can't be more than 3 characters long, please use abreviation for the state!");
        if (postcode != null && postcode.Length != 4)
            ModelState.AddModelError(nameof(postcode), "Postcode can't be more than 4 characters long.");
        if (mobile != null && mobile.Length > 12)
            ModelState.AddModelError(nameof(city), "Mobile number too long you can only write it with a max of 12 characters.");
        if (!ModelState.IsValid)
        {
            ViewBag.Name = name;
            ViewBag.Tfn = tfn;
            ViewBag.Address = address;
            ViewBag.City = city;
            ViewBag.State = state;
            ViewBag.PostCode = postcode;
            ViewBag.Mobile = mobile;
            return View(id);
        }
        var customer = new Customer
        {
            CustomerID = id,
            Name = name,
            TFN = tfn,
            Address = address,
            City = city,
            State = state,
            PostCode = postcode,
            Mobile = mobile
        };
        var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
        var response = await Client.PutAsync($"api/Customers/", content);
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        return RedirectToAction(nameof(UserSelect));
    }
    //Method to lock login - currently not functioning due to exception error
    public async Task<IActionResult> LockLogin(int id)
    {
        var response = await Client.GetAsync("api/Logins/");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var logins = JsonConvert.DeserializeObject<List<Login>>(result);
        Login login = null;
        for( int i = 0; i < logins.Count; i++ )
        {
            if( logins[i].CustomerID == id)
            {
                login = logins[i];
            }
        }

        if(login != null && login.IsLocked == true)
        {
            login.IsLocked = false;
        }
        else
        {
            login.IsLocked = true;
        }
        if(login.LoginID.GetType() == typeof(int))
        {
            Console.WriteLine("It is an int");
        }
        var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
        response = await Client.PutAsync($"api/Login/", content);
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        return RedirectToAction(nameof(UserSelect));
    }
    //Allows selection of account for transactions webpage
    public async Task<IActionResult> AccountSelect(int id)
    {
        var response = await Client.GetAsync($"api/Account/Customer/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var accounts = JsonConvert.DeserializeObject<List<Account>>(result);

        return View(accounts);
    }
    public IActionResult DateEnter(Account account)
    {
        return View(account);
    }

    //Method to get the transactions - doesnt work due to account not being passed through properly.
    [HttpPost]
    public async Task<IActionResult> DateEnter(Account account, DateTime Start, DateTime End)
    {
        List<Transaction> transactions;
        var startUTC = Start.ToUniversalTime();
        var endUTC = End.ToUniversalTime();
        Console.WriteLine(Start);

        var response = await Client.GetAsync($"api/Transaction/Customer/{account.AccountNumber}");
        if (!response.IsSuccessStatusCode)
            throw new Exception();
        var result = await response.Content.ReadAsStringAsync();
        transactions = JsonConvert.DeserializeObject<List<Transaction>>(result);
        
        return RedirectToAction(nameof(ViewTransactions), new {transactions = transactions, start = startUTC, end = endUTC});
    }
    public IActionResult ViewTransactions(List<Transaction> transactions, DateTime start, DateTime end)
    {
        ViewBag.start = start;
        ViewBag.end = end;
        return View(transactions);
    }

    public async Task<IActionResult> BillPayView()
    {
        var response = await Client.GetAsync($"api/Billpay");
        if (!response.IsSuccessStatusCode)
            throw new Exception();
        var result = await response.Content.ReadAsStringAsync();
        var billPays = JsonConvert.DeserializeObject<List<BillPay>>(result);
        return View(billPays);
    }
    //allows to lock billpay entries.
    public async Task<IActionResult> BillPayLock(int id)
    {
        var response = await Client.GetAsync($"api/Billpay/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var billPay = JsonConvert.DeserializeObject<BillPay>(result);
        if (billPay.IsLocked == true)
        {
            billPay.IsLocked = false;
        }
        else
        {
            billPay.IsLocked = true;
        }
        var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");
        response = await Client.PutAsync($"api/Billpay/", content);
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        return RedirectToAction(nameof(UserSelect));
    }

}
