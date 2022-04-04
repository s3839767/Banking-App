using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountManager _repo;

    public AccountController(AccountManager repo)
    {
        _repo = repo;
    }

    // GET: api/movies
    [HttpGet]
    public IEnumerable<Account> Get()
    {
        return _repo.GetAll();
    }

    [HttpGet("Customer/{id}")]
    public IEnumerable<Account> GetAllForOne(int id)
    {
        return _repo.GetAllForCustomer(id);
    }

    // GET api/movies/1
    [HttpGet("{id}")]
    public Account Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/movies
    [HttpPost]
    public void Post([FromBody] Account account)
    {
        _repo.Add(account);
    }

    //PUT api/movies
   [HttpPut]
    public void Put([FromBody] Account account)
    {
        _repo.Update(account.AccountNumber, account);
    }

    // DELETE api/movies/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
