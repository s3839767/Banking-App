using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public enum Period
    {
        One_Off = 'O',
        Monthly = 'M'
    }

    public class BillPay
    {
        [Required]
        public int BillPayID { get; set; }

        [ForeignKey("Account"), Required]
        public int AccountNumber { get; set; }

        public virtual Account Account { get; set; }

        [ForeignKey("Payee"), Required]
        public int PayeeID { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScehduleTimeUtc { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [Required]
        public char Period { get; set; }
    }
}