using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly TransactionManager _repo;

    public TransactionController(TransactionManager repo)
    {
        _repo = repo;
    }

    // GET: api/movies
    [HttpGet]
    public IEnumerable<Transaction> Get()
    {
        return _repo.GetAll();
    }
    [HttpGet("Customer/{id}")]
    public IEnumerable<Transaction> GetAllCustomer(int id)
    {
        return _repo.GetAllCustomer(id);
    }
    //[HttpGet("{id}")]
    //public IEnumerable<Transaction> GetStart(int id, DateTime start)
    //{
    //    return _repo.GetStart(id, start);
    //}
    //[HttpGet("{id}/{end}")]
    //public IEnumerable<Transaction> GetEnd(int id, DateTime end)
    //{
    //    return _repo.GetEnd(id, end);
    //}

    //[HttpGet("{id}/{start}/{end}")]
    //public IEnumerable<Transaction> GetBetween(int id, DateTime start, DateTime end)
    //{
    //    return _repo.GetBetween(id, start, end);
    //}

    // GET api/movies/1
    [HttpGet("{id}")]
    public Transaction Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/movies
    [HttpPost]
    public void Post([FromBody] Transaction transaction)
    {
        _repo.Add(transaction);
    }

    //PUT api/movies
   [HttpPut]
    public void Put([FromBody] Transaction transaction)
    {
        _repo.Update(transaction.TransactionID, transaction);
    }

    // DELETE api/movies/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
