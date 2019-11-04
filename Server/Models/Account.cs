using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Studio1BTask.Models
{
    public class Account
    {
        [Key] public int Id { get; set; }
        public char Type { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public virtual Session Session { get; set; }
        [ForeignKey("Session")] public int? SessionId { get; set; }
    }
}