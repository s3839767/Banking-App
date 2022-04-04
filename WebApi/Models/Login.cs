using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Login
{
    [Column(TypeName = "char")]
    [Required, StringLength(8)]
    public int LoginID { get; set; }

    [Required]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(64)]
    public string PasswordHash { get; set; }

    [Required]
    public bool IsLocked { get; set; }
}