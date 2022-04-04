using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-6.0

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillPayManager _repo;

    public BillPayController(BillPayManager repo)
    {
        _repo = repo;
    }

    // GET: api/movies
    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }

    // GET api/movies/1
    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/movies
    [HttpPost]
    public void Post([FromBody] BillPay billPay)
    {
        _repo.Add(billPay);
    }

    //PUT api/movies
   [HttpPut]
    public void Put([FromBody] BillPay billPay)
    {
        _repo.Update(billPay.BillPayID, billPay);
    }

    // DELETE api/movies/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
