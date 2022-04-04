using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class LoginsController : ControllerBase
{
    private readonly LoginManager _repo;

    public LoginsController(LoginManager repo)
    {
        _repo = repo;
    }

    // GET: api/movies
    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    // GET api/movies/1
    [HttpGet("{id}")]
    public Login Get(int id)
    {
        return _repo.Get(id);
    }
    [HttpGet("/Customer/{id}")]
    public Login GetByCustomer(int id)
    {
        return _repo.GetByCustomer(id);
    }

    // POST api/movies
    [HttpPost]
    public void Post([FromBody] Login login)
    {
        _repo.Add(login);
    }

    //PUT api/movies
   [HttpPut]
    public void Put([FromBody] Login login)
    {
        _repo.Update((int)login.LoginID, login);
    }

    // DELETE api/movies/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
