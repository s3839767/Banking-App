using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;


public class Transaction
{
    [Required]
    public int TransactionID { get; set; }
    [Required]
    public char TransactionType { get; set; }

    [Required, ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Required, Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }
    [Required]
    public DateTime TransactionTimeUtc { get; set; }
}