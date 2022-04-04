using WebApi.Data;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class LoginManager : IDataRepository<Login, int>
{
    private readonly McbaContext _context;

    public LoginManager(McbaContext context)
    {
        _context = context;
    }

    public Login Get(int id)
    {
        return _context.Logins.Find(id);
    }

    public Login GetByCustomer(int id)
    {
        return _context.Logins.Where(x => x.CustomerID == id).First();
    }

    public IEnumerable<Login> GetAll()
    {
        return _context.Logins.ToList();
    }

    public int Add(Login login)
    {
        _context.Logins.Add(login);
        _context.SaveChanges();

        return login.LoginID;
    }

    public int Delete(int id)
    {
        _context.Logins.Remove(_context.Logins.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Login login)
    {
        _context.Update(login);
        _context.SaveChanges();
            
        return id;
    }
}
