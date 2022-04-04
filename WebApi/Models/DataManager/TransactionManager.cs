using WebApi.Data;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class TransactionManager : IDataRepository<Transaction, int>
{
    private readonly McbaContext _context;

    public TransactionManager(McbaContext context)
    {
        _context = context;
    }

    public Transaction Get(int id)
    {
        return _context.Transactions.Find(id);
    }
    public IEnumerable<Transaction> GetAllCustomer(int id)
    {
        return _context.Transactions.Where(x => x.AccountNumber == id).ToList();
    }
    //public IEnumerable<Transaction> GetBetween(int id, DateTime start, DateTime end)
    //{
    //    return _context.Transactions.Where(x => x.AccountNumber == id && x.TransactionTimeUtc >= start && x.TransactionTimeUtc <= end).ToList();
    //}

    //public IEnumerable<Transaction> GetEnd(int id, DateTime end)
    //{
    //    return _context.Transactions.Where(x => x.AccountNumber == id && x.TransactionTimeUtc <= end).ToList();
    //}

    //public IEnumerable<Transaction> GetStart(int id, DateTime start)
    //{
    //    return _context.Transactions.Where(x => x.AccountNumber == id && x.TransactionTimeUtc >= start).ToList();
    //}

    public IEnumerable<Transaction> GetAll()
    {
        return _context.Transactions.ToList();
    }

    public int Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return transaction.TransactionID;
    }

    public int Delete(int id)
    {
        _context.Transactions.Remove(_context.Transactions.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Transaction transaction)
    {
        _context.Update(transaction);
        _context.SaveChanges();
            
        return id;
    }
}
