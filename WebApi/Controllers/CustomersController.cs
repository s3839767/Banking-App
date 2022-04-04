using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerManager _repo;

    public CustomersController(CustomerManager repo)
    {
        _repo = repo;
    }

    // GET: api/movies
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _repo.GetAll();
    }

    // GET api/movies/1
    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/movies
    [HttpPost]
    public void Post([FromBody] Customer customer)
    {
        _repo.Add(customer);
    }

    // PUT api/movies
    [HttpPut]
    public void Put([FromBody] Customer customer)
    {
        _repo.Update(customer.CustomerID, customer);
    }

    // DELETE api/movies/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
