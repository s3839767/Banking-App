
using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using Microsoft.EntityFrameworkCore;

namespace McbaExampleWithLogin.BackgroundServices
{
    // class taken from Week 8 Lectorial 
    public class CustomerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<CustomerBackgroundService> _logger;

        public CustomerBackgroundService(IServiceProvider services, ILogger<CustomerBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Background Service is running.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);

                _logger.LogInformation("Customer Background Service is waiting a minute.");

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Background Service is working.");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

            var bills = await context.BillPay.Where(x => x.ScehduleTimeUtc <= DateTime.UtcNow && x.IsLocked == false).ToListAsync(cancellationToken);

            if (bills.Any())
            {
                foreach (var bill in bills)
                {
                    var account = await context.Accounts.FindAsync(bill.AccountNumber);
                    var amountDue = bill.Amount;

                    if (account.Balance >= amountDue && !(account.AccountType.Equals('C') && account.Balance - amountDue < 300))
                    {
                        account.Balance -= amountDue;
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionType = char.Parse("B"),
                                Amount = amountDue,
                                TransactionTimeUtc = DateTime.UtcNow
                            });
                        if (bill.Period.Equals('M'))
                        {
                            bill.ScehduleTimeUtc = bill.ScehduleTimeUtc.AddMonths(1);
                        }
                        else if (bill.Period.Equals('O'))
                        {
                            context.Remove(bill);
                        }
                    }
                    else
                    {
                        context.Remove(bill);
                    }

                }
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer Background Service work complete.");
        }
    }
}
