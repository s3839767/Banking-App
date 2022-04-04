using WebApi.Data;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class BillPayManager : IDataRepository<BillPay, int>
{
    private readonly McbaContext _context;

    public BillPayManager(McbaContext context)
    {
        _context = context;
    }

    public BillPay Get(int id)
    {
        return _context.BillPay.Find(id);
    }

    public IEnumerable<BillPay> GetAll()
    {
        return _context.BillPay.ToList();
    }

    public int Add(BillPay billpay)
    {
        _context.BillPay.Add(billpay);
        _context.SaveChanges();

        return billpay.BillPayID;
    }

    public int Delete(int id)
    {
        _context.BillPay.Remove(_context.BillPay.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, BillPay billPay)
    {
        _context.Update(billPay);
        _context.SaveChanges();
            
        return id;
    }
}
