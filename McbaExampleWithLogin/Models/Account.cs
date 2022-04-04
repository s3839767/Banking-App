using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaExampleWithLogin.Models;



public class Account
{
    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Required, Display(Name = "Type")]
    public char AccountType { get; set; }

    [Required]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    //[InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    public virtual List<BillPay> BillPays { get; set; }
}